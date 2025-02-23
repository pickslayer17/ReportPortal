﻿using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IFolderService
    {
        public Task<int> GetIdOrAddFolderInRun(int runId, string path);
        public Task AttachTestToFolder(int folderId, int testId);
        public Task<FolderDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        public Task<IEnumerable<FolderDto>> GetAllFolders(int runId, CancellationToken cancellationToken = default);
        public Task DeleteFolder(int folderId);
        public Task<int> DoesFolderExists(int runId, string path);
    }
}
