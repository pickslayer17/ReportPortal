using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IProjectRepository _projectRepostitory;
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;

        public ProjectService(IFolderRepository folderRepository, IProjectRepository projectRepostitory, ITestRepository testRepository, ITestResultRepository testResultRepository)
        {
            _folderRepository = folderRepository;
            _projectRepostitory = projectRepostitory;
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
        }

        public Task<ProjectCreatedDto> CreateAsync(ProjectForCreationDto accountForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allModels = await _projectRepostitory.GetAllByAsync(pr => pr.Id != 0);

            return allModels.Select(x => new ProjectDto { Id = x.Id });
        }

        public Task<IEnumerable<ProjectDto>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
