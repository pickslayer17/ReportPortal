using ReportPortal.BL.Models.TrxModels;


namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITrxParserService
    {
        // failedOnly: when true, only tests whose outcome maps to Failed are imported (passed/not-run skipped).
        public Task AddTestsFromXml(string xmlFilePath, int runId = default, bool failedOnly = false, CancellationToken cancellationToken = default);
    }
}