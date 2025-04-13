using AutoMapper;
using ReportPortal.BL.Constatnts;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

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
        private readonly IMapper _mapper;

        public RunService(
            IRunRepository runRepository,
            IFolderRepository folderRepository,
            ITestRepository testRepository,
            ITestResultRepository testResultRepository,
            IProjectRepository projectRepository,
            IMapper mapper,
            IFolderService folderService)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
            _folderService = folderService;
        }

        public async Task<RunCreatedDto> CreateAsync(RunDto runForCreationDto, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(p => p.Id ==  runForCreationDto.ProjectId);
            if (project == null) throw new ProjectNotFoundException($"no project with such id: {runForCreationDto.ProjectId}");

            var run = _mapper.Map<Run>(runForCreationDto);
            
            var runId = await _runRepository.InsertAsync(run);

            var runCreatedDto = new RunCreatedDto
            {
                Id = runId,
            };

            await _folderService.CreateRootFolderAsync(runCreatedDto.Id, cancellationToken);

            return runCreatedDto;
        }

        public async Task DeleteByIdAsync(int runId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RunDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
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
