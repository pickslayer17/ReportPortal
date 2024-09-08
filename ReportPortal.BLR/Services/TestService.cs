using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
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
        private readonly IFolderService _folderService;
        private readonly IFolderRepository _folderRepository;

        public TestService(ITestRepository testRepository, IMapper mapper, IFolderService folderService, ITestResultRepository testResultRepository, IFolderRepository folderRepository)
        {
            _testRepository = testRepository;
            _mapper = mapper;
            _folderService = folderService;
            _testResultRepository = testResultRepository;
            _folderRepository = folderRepository;
        }

        public async Task<TestCreatedDto> CreateAsync(TestDto testDto, int folderId, CancellationToken cancellationToken = default)
        {
            /// verify if test with such name already exists
            var folder = await _folderService.GetByIdAsync(folderId);
            if (folder.TestIds != null && folder.TestIds.Count > 0)
            {
                foreach (var testId in folder.TestIds)
                {
                    var test = await _testRepository.GetByAsync(t => t.Id == testId);
                    if (test.Name == testDto.Name) throw new TestWithSuchNameAlreadyExists($"Test with name '{testDto.Name}' already exists in folder with id {folderId}");
                }
            }

            // insert test to databse
            var testRunItem = _mapper.Map<Test>(testDto);
            testRunItem.FolderId = folderId;
            var testCreatedId = await _testRepository.InsertAsync(testRunItem);
            var testCreated = _mapper.Map<TestCreatedDto>(testRunItem);
            testCreated.Id = testCreatedId;

            return testCreated;
        }

        public Task<TestCreatedDto> CreateAsync(TestDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
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
            var folder = await _folderService.GetByIdAsync(folderId);
            var testIds = folder.TestIds;

            var tests = new List<TestDto>();
            if (testIds != null)
            {
                foreach (var testId in folder.TestIds)
                {
                    var test = await GetByIdAsync(testId);
                    tests.Add(_mapper.Map<TestDto>(test));
                }
            }

            return tests;
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
