using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.Hubs;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestReviewManagementController : ControllerBase
    {
        private readonly ITestReviewService _testReviewService;
        private readonly ITestService _testService;
        private readonly IMapper _mapper;
        private readonly IHubContext<RunUpdatesHub> _hubContext;

        public TestReviewManagementController(ITestReviewService testReviewService, IMapper mapper, ITestService testService, IHubContext<RunUpdatesHub> hubContext)
        {
            _testReviewService = testReviewService;
            _mapper = mapper;
            _testService = testService;
            _hubContext = hubContext;
        }

        [HttpGet("test/{testId:int}/TestReview")]
        [Authorize]
        public async Task<IActionResult> GetTestReview(int testId, CancellationToken cancellationToken = default)
        {
            var testReviewDto = await _testReviewService.GetTestReviewAsync(testId, cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("UpdateTestReview")]
        [Authorize]
        public async Task<IActionResult> UpdateTestReview([FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewDto = _mapper.Map<TestReviewDto>(testReview);
            var testReviewDtoUpdated = await _testReviewService.UpdateTestReviewAsync(testReviewDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDtoUpdated));
        }

        [HttpPut("TestReview/{id:int}/UpdateReviewer")]
        [Authorize]
        public async Task<IActionResult> UpdateReviewer(int id, [FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                ReviewerId = new Optional<int?>(testReview.ReviewerId)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }


        [HttpPut("TestReview/{id:int}/UpdateOutcome")]
        [Authorize]
        public async Task<IActionResult> UpdateOutcome(int id, [FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                TestReviewOutcome = new Optional<TestReviewOutcome>(testReview.TestReviewOutcome),
                ProductBug = testReview.ProductBug
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("TestReview/{id:int}/UpdateComments")]
        [Authorize]
        public async Task<IActionResult> UpdateComments(int id, [FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                Comments = new Optional<string?>(testReview.Comments)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }
    }
}
