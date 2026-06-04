using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Caching;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class RunService : IRunService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFolderService _folderService;
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IRunRepository _runRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFolderTreeCache _folderTreeCache;
        private readonly IMapper _mapper;

        public RunService(
            IRunRepository runRepository,
            IFolderRepository folderRepository,
            ITestRepository testRepository,
            ITestResultRepository testResultRepository,
            IProjectRepository projectRepository,
            IUnitOfWork unitOfWork,
            IFolderTreeCache folderTreeCache,
            IMapper mapper,
            IFolderService folderService)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
            _projectRepository = projectRepository;
            _unitOfWork = unitOfWork;
            _folderTreeCache = folderTreeCache;
            _mapper = mapper;
            _folderService = folderService;
        }

        public async Task<RunDto> CreateAsync(RunDto runForCreationDto, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(p => p.Id == runForCreationDto.ProjectId);
            if (project == null) throw new ProjectNotFoundException($"no project with such id: {runForCreationDto.ProjectId}");

            var run = _mapper.Map<Run>(runForCreationDto);

            RunDto runCreatedDto = null;

            // Run + its root folder must be created together or not at all.
            await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                var runId = await _runRepository.InsertAsync(run, ct);
                var rootFolderId = await _folderService.CreateRootFolderAsync(runId, ct);

                runCreatedDto = new RunDto
                {
                    Id = runId,
                    ProjectId = project.Id,
                    Name = runForCreationDto.Name,
                    RootFolderId = rootFolderId,
                };
            }, cancellationToken);

            return runCreatedDto;
        }

        public async Task DeleteByIdAsync(int runId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.ExecuteInTransactionAsync(
                ct => _runRepository.RemoveRunCascadeAsync(runId, ct),
                cancellationToken);

            // Run and all its folders are gone -> drop its cached tree.
            _folderTreeCache.Invalidate(runId);
        }

        public Task<IEnumerable<RunDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RunDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default)
        {
            // Filter pushed to SQL (WHERE ProjectId = @p) instead of loading every run and filtering in memory.
            var runs = await _runRepository.GetAllByAsync(r => r.ProjectId == projectId, cancellationToken);
            return runs.Select(rm => _mapper.Map<RunDto>(rm));
        }

        public async Task<IEnumerable<RunDto>> GetAllByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var allRunModels = await _runRepository.GetAllByAsync(r => true, cancellationToken);
            var resultDto = allRunModels.Select(rm => _mapper.Map<RunDto>(rm)).Where(predicate.Compile());

            return resultDto;
        }

        public Task<RunDto> GetByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<RunDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var run = await _runRepository.GetByAsync(r => r.Id == id, cancellationToken);
            return _mapper.Map<RunDto>(run);
        }
    }

}
