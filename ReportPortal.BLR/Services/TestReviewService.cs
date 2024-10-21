using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.BL.Services
{
    public class TestReviewService : ITestReviewService
    {
        private readonly ITestReviewRepository _testReviewRepository;
        private readonly IMapper _mapper;

        public TestReviewService(ITestReviewRepository testReviewRepository, IMapper mapper)
        {
            _testReviewRepository = testReviewRepository;
            _mapper = mapper;
        }

        public async Task<TestReviewDto> GetTestReviewAsync(int testId, CancellationToken cancellationToken = default)
        {
            var testReview = await _testReviewRepository.GetByAsync(tr => tr.TestId == testId, cancellationToken);

            return _mapper.Map<TestReviewDto>(testReview);
        }

        public async Task UpdateTestReviewAsync(TestReviewDto testReviewDto, CancellationToken cancellationToken = default)
        {
            await _testReviewRepository.UpdateItem(_mapper.Map<TestReview>(testReviewDto));
        }
    }
}
