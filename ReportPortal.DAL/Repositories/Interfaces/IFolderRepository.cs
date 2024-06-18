﻿using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IFolderRepository : IRepository<Folder>
    {
        public Task UpdateItem(Folder item);
    }
}
