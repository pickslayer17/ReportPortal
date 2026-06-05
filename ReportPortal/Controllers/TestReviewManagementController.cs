using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.Authorization;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Repositories.Interfaces;
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
        private readonly IProjectScopeRepository _scope;

        public TestReviewManagementController(ITestReviewService testReviewService, IMapper mapper, ITestService testService, IHubContext<RunUpdatesHub> hubContext, IProjectScopeRepository scope)
        {
            _testReviewService = testReviewService;
            _mapper = mapper;
            _testService = testService;
            _hubContext = hubContext;
            _scope = scope;
        }

        [HttpGet("test/{testId:int}/TestReview")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetTestReview(int testId, CancellationToken cancellationToken = default)
        {
            var testReviewDto = await _testReviewService.GetTestReviewAsync(testId, cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("UpdateTestReview")]
        [Authorize(Policy = Permissions.ReviewTests)]
        public async Task<IActionResult> UpdateTestReview([FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var reviewerId = CurrentUserId();
            if (reviewerId == null) return Unauthorized();

            // reviewId is in the body, not the route: guard membership here.
            await ScopeGuard.EnsureAccessAsync(User, _scope, ScopeResource.TestReview, testReview.Id, cancellationToken);

            // Targeted update (no blanket SetValues) and the reviewer is the authenticated user, not the body.
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = testReview.Id,
                ReviewerId = new Optional<int?>(reviewerId),
                Comments = new Optional<string?>(testReview.Comments),
                TestReviewOutcome = new Optional<TestReviewOutcome>(testReview.TestReviewOutcome),
                ProductBug = testReview.ProductBug
            };
            var testReviewDtoUpdated = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDtoUpdated.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDtoUpdated));
        }

        // Any project member can assign any project member as the reviewer (no need to be that
        // user). The caller's access to the review's project is enforced by ProjectScopeFilter;
        // here we additionally require the assigned reviewer to belong to the same project.
        [HttpPut("TestReview/{reviewId:int}/UpdateReviewer/{reviewerId:int}")]
        [Authorize(Policy = Permissions.ReviewTests)]
        public async Task<IActionResult> UpdateReviewer(int reviewId, int reviewerId, CancellationToken cancellationToken = default)
        {
            // Reviewer must belong to the SUBPROJECT that owns this review (subproject-scoped eligibility).
            var subprojectId = await _scope.ResolveSubprojectIdForReviewAsync(reviewId, cancellationToken);
            if (subprojectId == null) return NotFound();
            if (!await _scope.IsSubprojectMemberAsync(reviewerId, subprojectId.Value, cancellationToken))
                return BadRequest(new { message = "Reviewer must be a member of the subproject." });

            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = reviewId,
                ReviewerId = new Optional<int?>(reviewerId)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }


        [HttpPut("TestReview/{reviewId:int}/UpdateOutcome")]
        [Authorize(Policy = Permissions.ReviewTests)]
        public async Task<IActionResult> UpdateOutcome(int reviewId, [FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = reviewId,
                TestReviewOutcome = new Optional<TestReviewOutcome>(testReview.TestReviewOutcome),
                ProductBug = testReview.ProductBug
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("TestReview/{reviewId:int}/UpdateComments")]
        [Authorize(Policy = Permissions.ReviewTests)]
        public async Task<IActionResult> UpdateComments(int reviewId, [FromBody] TestReviewVm testReview, CancellationToken cancellationToken = default)
        {
            var testReviewUpdateDto = new TestReviewUpdateDto
            {
                Id = reviewId,
                Comments = new Optional<string?>(testReview.Comments)
            };
            var testReviewDto = await _testReviewService.UpdateTestReviewAsync(testReviewUpdateDto, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testReviewDto.TestId, cancellationToken);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        private int? CurrentUserId()
        {
            return int.TryParse(User.FindFirst("UserId")?.Value, out var id) ? id : null;
        }
    }
}
