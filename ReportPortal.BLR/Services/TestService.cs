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

        public TestService(ITestRepository testRepository, IMapper mapper, IFolderService folderService, ITestResultRepository testResultRepository)
        {
            _testRepository = testRepository;
            _mapper = mapper;
            _folderService = folderService;
            _testResultRepository = testResultRepository;
        }

        public async Task<TestCreatedDto> CreateAsync(TestDto testDto, CancellationToken cancellationToken = default)
        {
            // create or take existing folder
            var folderId = await _folderService.GetIdOrAddFolderInRun(testDto.RunId, testDto.Path);
            testDto.FolderId = folderId;

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
            var testRunItem = _mapper.Map<TestRunItem>(testDto);
            var testCreatedId = await _testRepository.InsertAsync(testRunItem);
            var testCreated = _mapper.Map<TestCreatedDto>(testRunItem);
            testCreated.Id = testCreatedId;

            // Set test to folder
            await _folderService.AttachTestToFolder(folderId, testCreated.Id);

            return testCreated;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var test = await GetByIdAsync(id);
            if (test.TestResultIds != null)
            {
                foreach (var testResultId in test.TestResultIds)
                {
                    var testResult = await _testResultRepository.GetByAsync(tr => tr.Id == testResultId);
                    await _testResultRepository.RemoveAsync(_mapper.Map<TestResult>(testResult));
                }
            }

            var folder = await _folderService.GetByIdAsync(test.FolderId);
            if (folder != null && folder.TestIds != null)
            {
                var isRemoved = folder.TestIds.Remove(test.Id);
                if (!isRemoved) throw new Exception("Would be nice to add logger here, but let it be exception for a while. Test wasnt present in folder test ids list");
            }
            ///_folderService.Update(folder);

            await _testRepository.RemoveAsync(_mapper.Map<TestRunItem>(test));
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
            if (test == null) throw new TestNotFoundExeption($"Test with id {id} was not found");

            return _mapper.Map<TestDto>(test);
        }
    }

}
