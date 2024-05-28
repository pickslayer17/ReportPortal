﻿using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IFolderRepository : IRepository<FolderRunItem>
    {
        public Task UpdateItem(FolderRunItem item);
    }
}
