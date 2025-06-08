using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Helpers;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.TrxModels;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.BL.Services.Interfaces
{
    public class TrxParserService : ITrxParserService
    {
        private readonly ITestService _testService;
        private readonly IFolderService _folderService;
        private readonly ITestResultService _testResultService;

        public TrxParserService(ITestService testService, IFolderService folderService, ITestResultService testResultService)
        {
            _testService = testService;
            _folderService = folderService;
            _testResultService = testResultService;
        }

        public async Task AddTestsFromXml(string xmlFilePath, bool isNeedToRemovePassed = true, int runId = default, CancellationToken cancellationToken = default)
        {
            string xml;
            using (var reader = new StreamReader(xmlFilePath))
            {
                xml = await reader.ReadToEndAsync();
            }

            var tests = TrxHelper.GetTestsFromTrxXml(xml, isNeedToRemovePassed, runId);

            runId = 2;
           

            foreach (var test in tests)
            {
                var folderPath = test.FullName.Substring(0, test.FullName.LastIndexOf('.'));
                var testDto = new TestDto
                {
                    Name = test.Name,
                    RunId = runId,
                    Path = folderPath,

                    
                };
                var folderId = await _folderService.GetIdOrAddFolderInRunAsync(runId, folderPath, cancellationToken);
                var testCreated = await _testService.CreateAsync(testDto, folderId, cancellationToken);
                var testResultDto = new TestResultDto
                {
                    TestId = testCreated.Id,
                    RunId = runId,
                    ErrorMessage = test.Message?? string.Empty,
                    StackTrace = test.StackTrace?? string.Empty,
                    TestOutcome = GetOutcome(test.Outcome),
                };

                await _testResultService.AddTestResultToTestAsync(testCreated.Id, testResultDto, cancellationToken);
            }
        }

        private TestOutcome GetOutcome(string nUnitOutcome)
        {
            switch(nUnitOutcome)
            {
                case TrxTestOutcome.Passed:
                    return TestOutcome.Passed;
                    break;
                case TrxTestOutcome.Failed:
                    return TestOutcome.Failed;
                    break;
                case TrxTestOutcome.NotExecuted:
                    return TestOutcome.NotRun;
                    break;
                default:
                    throw new NotImplementedException();


            }
        }
    }
}