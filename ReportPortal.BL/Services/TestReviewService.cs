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

        public async Task<TestReviewDto> UpdateTestReviewAsync(TestReviewDto testReviewDto, CancellationToken cancellationToken = default)
        {
            var testReviewUpdated = await _testReviewRepository.UpdateItemAsync(_mapper.Map<TestReview>(testReviewDto), cancellationToken);

            return _mapper.Map<TestReviewDto>(testReviewUpdated);
        }

        public async Task<TestReviewDto> UpdateTestReviewAsync(TestReviewUpdateDto testReviewUpdateDto, CancellationToken cancellationToken = default)
        {
            var testReview = await _testReviewRepository.GetByAsync(testReview => testReview.Id == testReviewUpdateDto.Id, cancellationToken);
            if (testReview == null) throw new Exception($"TestReview  with ID {testReviewUpdateDto.Id} not found");

            // Apply updates from the DTO
            if (testReviewUpdateDto.ReviewerId.HasValue)
                testReview.ReviewerId = testReviewUpdateDto.ReviewerId.Value;

            if (testReviewUpdateDto.Comments.HasValue)
                testReview.Comments = testReviewUpdateDto.Comments.Value;

            if (testReviewUpdateDto.TestReviewOutcome.HasValue)
            {
                testReview.TestReviewOutcome = testReviewUpdateDto.TestReviewOutcome.Value;
                if (testReview.TestReviewOutcome == DAL.Enums.TestReviewOutcome.ProductBug)
                    testReview.ProductBug = testReviewUpdateDto.ProductBug;
            }

            // Save the updated domain model
            var testReviewUpdated = await _testReviewRepository.UpdateItemAsync(testReview, cancellationToken);
            return _mapper.Map<TestReviewDto>(testReviewUpdated);
        }
    }
}
