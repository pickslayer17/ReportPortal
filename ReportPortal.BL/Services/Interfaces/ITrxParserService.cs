using ReportPortal.BL.Models.TrxModels;


namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITrxParserService
    {
        public Task AddTestsFromXml(string xmlFilePath, bool isNeedToRemovePassed = true, int runId = default, CancellationToken cancellationToken = default);
    }
}