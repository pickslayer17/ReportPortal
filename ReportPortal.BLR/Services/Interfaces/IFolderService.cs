namespace ReportPortal.BL.Services.Interfaces
{
    public interface IFolderService
    {
        public Task<int> GetIdOrAddFolderInRun(int runId, string path);
        public Task AttachTestToFolder(int folderId, int testId);
    }

}
