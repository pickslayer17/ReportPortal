﻿using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestService : IServiceBase<TestDto, TestCreatedDto>
    {
        public Task<TestCreatedDto> CreateAsync(TestDto projectForCreationDto, int folderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestDto>> GetAllByFolderIdAsync(int folderId, CancellationToken cancellationToken = default);
        public Task<IEnumerable<TestDto>> GetAllByRunIdAsync(int runId, CancellationToken cancellationToken = default);
        public Task<int> GetTestId(int folderId, string testName, CancellationToken cancellationToken = default);
    }
}
