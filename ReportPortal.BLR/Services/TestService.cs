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

        public TestService(ITestRepository testRepository, IAutoMapperService autoMapperService)
        {
            _testRepository = testRepository;
            _autoMapperService = autoMapperService;
        }

        public async Task<TestCreatedDto> CreateAsync(TestDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            var testRunItem = _autoMapperService.Map<TestDto, TestRunItem>(projectForCreationDto);
            var testCreatedId = await _testRepository.InsertAsync(testRunItem);

            var testCreated = _autoMapperService.Map<TestRunItem, TestCreatedDto>(testRunItem);
            testCreated.Id = testCreatedId;
            return testCreated;
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
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
    }

}
