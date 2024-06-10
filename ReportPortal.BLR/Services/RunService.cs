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
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IRunRepository _runRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public RunService(IRunRepository runRepository, IFolderRepository folderRepository, ITestRepository testRepository, ITestResultRepository testResultRepository, IProjectRepository projectRepository, IMapper mapper)
        {
            _runRepository = runRepository;
            _folderRepository = folderRepository;
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<RunCreatedDto> CreateAsync(RunDto runForCreationDto, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(pr => pr.Id == runForCreationDto.ProjectId);
            if (project == null)
            {
                throw new ProjectNotFoundException($"No project to create a Run in with id {runForCreationDto.ProjectId}");
            }

            var folderRunItem = new FolderRunItem
            {
                Name = FolderNames.RootFolderName,
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

            var rootFolder = await _folderRepository.GetByAsync(f => f.Id == rootFolderId);
            rootFolder.RunId = runId;
            await _folderRepository.UpdateItem(rootFolder);

            return new RunCreatedDto { Id = runId };
        }

        public Task<RunCreatedDto> CreateAsync(RunCreatedDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByIdAsync(int runId)
        {
            var folders = await _folderRepository.GetAllByAsync(f => f.RunId == runId);

            foreach (var folder in folders)
            {
                await _folderRepository.RemoveByIdAsync(folder.Id);
            }

            var tests = await _testRepository.GetAllByAsync(t => t.RunId == runId);
            foreach (var test in tests)
            {
                await _testRepository.RemoveByIdAsync(test.Id);
            }

            var testResults = await _testResultRepository.GetAllByAsync(tr => tr.RunId == runId);
            foreach (var tr in testResults)
            {
                await _testResultRepository.RemoveByIdAsync(tr.Id);
            }

            var run = await _runRepository.GetByAsync(r => r.Id == runId);
            await _runRepository.RemoveByIdAsync(run.Id);
        }

        public Task<IEnumerable<RunDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RunDto>> GetAllByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var allRunModels = await _runRepository.GetAllByAsync(r => true);
            var resultDto = allRunModels.Select(rm => _mapper.Map<RunDto>(rm)).Where(predicate.Compile());

            return resultDto;
        }

        public Task<RunDto> GetByAsync(Expression<Func<RunDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RunDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
