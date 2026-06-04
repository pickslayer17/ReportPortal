using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class TestRepository : AbstractApplicationRepository, ITestRepository
    {
        public TestRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<Test>> GetAllByAsync(Expression<Func<Test, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tests
                .Where(predicate)
                .Select(t => new Test
                {
                    Id = t.Id,
                    Name = t.Name,
                    RunId = t.RunId,
                    FolderId = t.FolderId,
                    TestResults = t.TestResults
                        .Select(tr => new TestResult
                        {
                            Id = tr.Id,
                            TestId = tr.TestId,
                            TestOutcome = tr.TestOutcome
                            // Не включаем ErrorMessage, StackTrace, ScreenShot!
                        }).ToList(),
                    TestReview = new TestReview
                    {
                        Id = t.TestReview.Id,
                        Comments = t.TestReview.Comments,
                        TestId = t.TestReview.TestId,
                        ProductBug = t.TestReview.ProductBug,
                        ReviewerId = t.TestReview.ReviewerId,
                        TestReviewOutcome = t.TestReview.TestReviewOutcome
                    }
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Test> GetByAsync(Expression<Func<Test, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var test =  await _dbContext.Tests.Include(t => t.TestResults).FirstOrDefaultAsync(predicate, cancellationToken);
            if (test == null) throw new TestNotFoundException($"There is no test with such predicate {predicate}");

            return test;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Test, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tests.AnyAsync(predicate, cancellationToken);
        }

        public async Task<List<Test>> GetByRunAsync(int runId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tests
                .AsNoTracking()
                .Where(t => t.RunId == runId)
                .Select(t => new Test { Id = t.Id, Name = t.Name, FolderId = t.FolderId })
                .ToListAsync(cancellationToken);
        }

        public async Task InsertRangeAsync(IEnumerable<Test> tests, CancellationToken cancellationToken = default)
        {
            await _dbContext.Tests.AddRangeAsync(tests, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<FolderTestStats>> GetFolderStatsByRunAsync(int runId, CancellationToken cancellationToken = default)
        {
            // Pull only the few columns we need (folder + latest outcome + review), never the full models.
            var rows = await _dbContext.Tests
                .AsNoTracking()
                .Where(t => t.RunId == runId)
                .Select(t => new
                {
                    t.FolderId,
                    Outcome = t.TestResults
                        .OrderByDescending(r => r.Id)
                        .Select(r => (TestOutcome?)r.TestOutcome)
                        .FirstOrDefault(),
                    Review = t.TestReview != null ? (TestReviewOutcome?)t.TestReview.TestReviewOutcome : null
                })
                .ToListAsync(cancellationToken);

            return rows
                .GroupBy(x => x.FolderId)
                .Select(g => new FolderTestStats
                {
                    FolderId = g.Key,
                    Total = g.Count(),
                    Passed = g.Count(x => x.Outcome == TestOutcome.Passed),
                    Failed = g.Count(x => x.Outcome == TestOutcome.Failed),
                    NotRun = g.Count(x => x.Outcome == TestOutcome.NotRun),
                    ToInvestigate = g.Count(x => x.Review == TestReviewOutcome.ToInvestigate),
                    NotRepro = g.Count(x => x.Review == TestReviewOutcome.NotRepro),
                    ProductBug = g.Count(x => x.Review == TestReviewOutcome.ProductBug),
                })
                .ToList();
        }

        public async Task<int> InsertAsync(Test testRunItem, CancellationToken cancellationToken = default)
        {
            _dbContext.Tests.Add(testRunItem);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return testRunItem.Id;
        }

        public async Task RemoveByIdAsync(int testId, CancellationToken cancellationToken = default)
        {
            var test = await GetByAsync(t => t.Id == testId, cancellationToken);
            _dbContext.Tests.Remove(test);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Test> UpdateItemAsync(Test item, CancellationToken cancellationToken = default)
        {
            var oldItem = await GetByAsync(t => t.Id == item.Id, cancellationToken);
            _dbContext.Tests.Entry(oldItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return item;
        }
    }
}
