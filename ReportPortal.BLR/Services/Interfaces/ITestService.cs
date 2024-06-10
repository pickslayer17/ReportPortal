﻿using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestService : IServiceBase<TestDto, TestCreatedDto>
    {
        public Task<IEnumerable<TestDto>> GetAllByFolderIdAsync(int folderId, CancellationToken cancellationToken = default);
    }
}
