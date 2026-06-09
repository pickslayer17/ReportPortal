using ReportPortal.BL.Helpers;
using ReportPortal.BL.Models.TrxModels;
using ReportPortal.BL.Services.Caching;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.BL.Services.Interfaces
{
    public class TrxParserService : ITrxParserService
    {
        private readonly IFolderService _folderService;
        private readonly ITestRepository _testRepository;
        private readonly ITestReviewRepository _testReviewRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFolderTreeCache _folderTreeCache;

        public TrxParserService(
            IFolderService folderService,
            ITestRepository testRepository,
            ITestReviewRepository testReviewRepository,
            ITestResultRepository testResultRepository,
            IUnitOfWork unitOfWork,
            IFolderTreeCache folderTreeCache)
        {
            _folderService = folderService;
            _testRepository = testRepository;
            _testReviewRepository = testReviewRepository;
            _testResultRepository = testResultRepository;
            _unitOfWork = unitOfWork;
            _folderTreeCache = folderTreeCache;
        }

        public async Task AddTestsFromXml(string xmlFilePath, int runId = default, bool failedOnly = false, CancellationToken cancellationToken = default)
        {
            string xml;
            using (var reader = new StreamReader(xmlFilePath))
            {
                xml = await reader.ReadToEndAsync();
            }

            var tests = TrxHelper.GetTestsFromTrxXml(xml, runId);

            // Optional filter: keep only failed tests. Done before folder resolution so empty
            // folders aren't materialised for tests we're dropping.
            if (failedOnly)
                tests = tests.Where(t => GetOutcome(t.Outcome) == TestOutcome.Failed).ToList();

            if (tests.Count == 0) return;

            // Folder path for a test = its full name minus the method name.
            var folderPathByTest = tests.ToDictionary(
                t => t,
                t => t.FullName.Substring(0, t.FullName.LastIndexOf('.')));

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async ct =>
                {
                    // 1) Resolve/create every folder in one batch (write-through to the tree cache).
                    var folderIdByPath = await _folderService.GetIdOrAddFoldersInRunAsync(
                        runId, folderPathByTest.Values.Distinct().ToList(), ct);

                    // 2) Dedup by (folder, name) against what's already stored and within this batch.
                    var existing = await _testRepository.GetByRunAsync(runId, ct);
                    var seen = new HashSet<(int folderId, string name)>(
                        existing.Select(e => (e.FolderId, e.Name.ToLower())));

                    var toInsert = new List<(UnitTestModel Source, Test Entity)>();
                    foreach (var test in tests)
                    {
                        var folderId = folderIdByPath[folderPathByTest[test]];
                        if (!seen.Add((folderId, test.Name.ToLower())))
                            continue; // duplicate name in the same folder -> skip

                        toInsert.Add((test, new Test { Name = test.Name, RunId = runId, FolderId = folderId }));
                    }

                    if (toInsert.Count == 0) return;

                    // 3) Batch-insert tests, then their reviews and results.
                    await _testRepository.InsertRangeAsync(toInsert.Select(x => x.Entity), ct);

                    var reviews = toInsert.Select(x => new TestReview { TestId = x.Entity.Id }).ToList();
                    var results = toInsert.Select(x => new TestResult
                    {
                        TestId = x.Entity.Id,
                        ErrorMessage = x.Source.Message ?? string.Empty,
                        StackTrace = x.Source.StackTrace ?? string.Empty,
                        TestOutcome = GetOutcome(x.Source.Outcome),
                    }).ToList();

                    await _testReviewRepository.InsertRangeAsync(reviews, ct);
                    await _testResultRepository.InsertRangeAsync(results, ct);
                }, cancellationToken);
            }
            catch
            {
                // The folder cache is written through before later steps; if the transaction
                // rolled back, drop the cached tree so it reloads from committed DB state.
                _folderTreeCache.Invalidate(runId);
                throw;
            }
        }

        private static TestOutcome GetOutcome(string trxOutcome)
        {
            // TRX outcomes are richer than our three buckets. Map known states; treat
            // anything unrecognized as Failed so problems surface rather than hide.
            switch (trxOutcome?.Trim().ToLowerInvariant())
            {
                case "passed":
                    return TestOutcome.Passed;

                case "failed":
                case "error":
                case "timeout":
                case "aborted":
                    return TestOutcome.Failed;

                case "notexecuted":
                case "inconclusive":
                case "warning":
                case "pending":
                case "notrunnable":
                case "disconnected":
                    return TestOutcome.NotRun;

                default:
                    return TestOutcome.Failed;
            }
        }
    }
}
