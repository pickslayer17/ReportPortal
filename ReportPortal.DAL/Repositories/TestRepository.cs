﻿using Microsoft.EntityFrameworkCore;
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
