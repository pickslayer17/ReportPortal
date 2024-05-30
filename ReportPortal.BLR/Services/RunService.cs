using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class RunService : IRunService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IRunRepository _runRepository;

        public RunService(IRunRepository runRepository, IFolderRepository folderRepository)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
        }

        public async Task<RunCreatedDto> CreateAsync(RunDto runForCreationDto, CancellationToken cancellationToken = default)
        {
            var folderRunItem = new FolderRunItem
            {
                Name = "Root",
                ParentId = null,
            };

            var rootFolderId = await _folderRepository.InsertAsync(folderRunItem);

            var run = new Run
            {
                Name = runForCreationDto.Name,
                ProjectId = runForCreationDto.ProjectId,
                RootFolderId = rootFolderId
            };
            var runId = await _runRepository.InsertAsync(run);

            return new RunCreatedDto { Id = runId };
        }

        public Task<RunCreatedDto> CreateAsync(RunCreatedDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RunDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RunDto>> GetAllByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RunDto> GetByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
