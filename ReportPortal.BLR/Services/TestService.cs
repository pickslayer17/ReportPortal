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
        private readonly IMapper _mapper;
        private readonly IFolderService _folderService;

        public TestService(ITestRepository testRepository, IMapper mapper, IFolderService folderService)
        {
            _testRepository = testRepository;
            _mapper = mapper;
            _folderService = folderService;
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

        public Task DeleteByIdAsync(int id)
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

        public Task<TestDto> GetByAsync(Expression<Func<TestDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

}
