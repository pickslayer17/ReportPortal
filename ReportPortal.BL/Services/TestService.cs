using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IMapper _mapper;
        private readonly IFolderRepository _folderRepository;
        private readonly ITestReviewRepository _testReviewRepository;

        public TestService(
            ITestRepository testRepository,
            IMapper mapper,
            ITestResultRepository testResultRepository,
            IFolderRepository folderRepository,
            ITestReviewRepository testReviewRepository)
        {
            _testRepository = testRepository;
            _mapper = mapper;
            _testResultRepository = testResultRepository;
            _folderRepository = folderRepository;
            _testReviewRepository = testReviewRepository;
        }

        public async Task<TestDto> CreateAsync(TestDto testDto, int folderId, CancellationToken cancellationToken = default)
        {
            // verify if test with such name already exists in the folder (checked in SQL,
            // so it works regardless of lazy/no-tracking loading of folder.Tests)
            var alreadyExists = await _testRepository.ExistsAsync(
                t => t.FolderId == folderId && t.Name == testDto.Name, cancellationToken);
            if (alreadyExists)
                throw new TestWithSuchNameAlreadyExists($"Test with name '{testDto.Name}' already exists in folder with id {folderId}");

            // insert test to databse
            var testRunItem = _mapper.Map<Test>(testDto);
            testRunItem.FolderId = folderId;
            var testCreatedId = await _testRepository.InsertAsync(testRunItem, cancellationToken);
            var testCreated = _mapper.Map<TestDto>(testRunItem);
            testCreated.Id = testCreatedId;
            var testReview = new TestReview();
            testReview.Test = testRunItem;
            await _testReviewRepository.InsertAsync(_mapper.Map<TestReview>(testReview), cancellationToken);

            return testCreated;
        }

        public async Task<int> GetTestIdAsync(int folderId, string testName, CancellationToken cancellationToken = default)
        {
            var folder = await _folderRepository.GetByAsync(f => f.Id == folderId, cancellationToken);
            if (folder == null) throw new FolderNotFoundException($"folder with id {folderId} not found");

            var test = folder.Tests.FirstOrDefault(t => t.Name.ToLower() == testName.ToLower());
            if (test == null) return - 1;

            return test.Id;
        }

        public Task<TestDto> CreateAsync(TestDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await _testRepository.RemoveByIdAsync(id, cancellationToken);
        }

        public Task<IEnumerable<TestDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestDto>> GetAllByAsync(Expression<Func<TestDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TestDto>> GetAllByFolderIdAsync(int folderId, CancellationToken cancellationToken = default)
        {
            var tests = await _testRepository.GetAllByAsync(t => t.FolderId == folderId, cancellationToken);
            return tests.Select(t => _mapper.Map<TestDto>(t));
        }

        public async Task<IEnumerable<TestDto>> GetAllByRunIdAsync(int runId, CancellationToken cancellationToken = default)
        {
            var tests = await _testRepository.GetAllByAsync(t => t.RunId == runId, cancellationToken);
            var testsDto = tests.Select(t => _mapper.Map<TestDto>(t));

            return testsDto;
        }

        public async Task<IEnumerable<FolderStatsDto>> GetFolderStatsByRunAsync(int runId, CancellationToken cancellationToken = default)
        {
            var stats = await _testRepository.GetFolderStatsByRunAsync(runId, cancellationToken);

            return stats.Select(s => new FolderStatsDto
            {
                FolderId = s.FolderId,
                Total = s.Total,
                Passed = s.Passed,
                Failed = s.Failed,
                NotRun = s.NotRun,
                ToInvestigate = s.ToInvestigate,
                NotRepro = s.NotRepro,
                ProductBug = s.ProductBug,
            });
        }

        public Task<TestDto> GetByAsync(Expression<Func<TestDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TestDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var test = await _testRepository.GetByAsync(t => t.Id == id, cancellationToken);
            if (test == null) throw new TestNotFoundException($"Test with id {id} was not found");

            return _mapper.Map<TestDto>(test);
        }
    }
}
