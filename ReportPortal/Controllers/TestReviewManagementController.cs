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
        public async Task<IActionResult> GetTestReview(int testId)
        {
            var testReviewDto = await _testReviewService.GetTestReviewAsync(testId);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("UpdateTestReview")]
        [Authorize]
        public async Task<IActionResult> UpdateTestReview([FromBody] TestReviewVm testReview)
        {
            var testReviewDto = _mapper.Map<TestReviewDto>(testReview);
            var testReviewDtoUpdated = await _testReviewService.UpdateTestReviewAsync(testReviewDto);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(_mapper.Map<TestReviewVm>(testReviewDtoUpdated));
        }

        [HttpPut("TestReview/{id:int}/UpdateReviewer/{reviewerId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateReviewer(int id, int? reviwerId)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                ReviewerId = new Optional<int?>(reviwerId)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }


        [HttpPut("TestReview/{id:int}/UpdateOutcome/{outcome:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateOutcome(int id, TestReviewOutcome outcome)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                TestReviewOutcome = new Optional<TestReviewOutcome>(outcome)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("TestReview/{id:int}/UpdateComments")]
        [Authorize]
        public async Task<IActionResult> UpdateComments(int id, [FromBody] string comments)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = id,
                Comments = new Optional<string?>(comments)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }
    }
}
