using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;
        private readonly IAutoMapperService _autoMapperService;
        private readonly IFolderService _folderService;

        public TestService(ITestRepository testRepository, IAutoMapperService autoMapperService, IFolderService folderService)
        {
            _testRepository = testRepository;
            _autoMapperService = autoMapperService;
            _folderService = folderService;
        }

        public async Task<TestCreatedDto> CreateAsync(TestDto testDto, CancellationToken cancellationToken = default)
        {
            // create or take existing folder
            var folderId = await _folderService.GetIdOrAddFolderInRun(testDto.RunId, testDto.Path);
            testDto.FolderId = folderId;

            // insert test to databse
            var testRunItem = _autoMapperService.Map<TestDto, TestRunItem>(testDto);
            var testCreatedId = await _testRepository.InsertAsync(testRunItem);
            var testCreated = _autoMapperService.Map<TestRunItem, TestCreatedDto>(testRunItem);
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
