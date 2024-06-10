using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class TestResultService : ITestResultService
    {
        private readonly ITestRepository _testRepository;
        private readonly ITestResultRepository _testResultRepository;
        private readonly IMapper _mapper;

        public TestResultService(ITestRepository testRepository, ITestResultRepository testResultRepository, IMapper mapper)
        {
            _testRepository = testRepository;
            _testResultRepository = testResultRepository;
            _mapper = mapper;
        }

        public async Task<int> AddTestResultToTest(int testId, TestResultDto testDto, CancellationToken cancellationToken = default)
        {
            var testResult = _mapper.Map<TestResult>(testDto);
            var test = await _testRepository.GetByAsync(t => t.Id == testId, cancellationToken);

            testResult.TestId = testId;
            testResult.RunId = test.RunId;
            var testResultId = await _testResultRepository.InsertAsync(testResult);

            if (test.TestResultIds == null) test.TestResultIds = new List<int>();
            test.TestResultIds.Add(testResultId);
            await _testRepository.UpdateItem(test);

            return testResultId;
        }

        public async Task<TestResultCreatedDto> CreateAsync(TestResultDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestResultDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestResultDto>> GetAllByAsync(Expression<Func<TestResultDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        public Task<TestResultDto> GetByAsync(Expression<Func<TestResultDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<TestResultDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
