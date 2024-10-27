using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestReviewManagementController : ControllerBase
    {
        private readonly ITestReviewService _testReviewService;
        private readonly IMapper _mapper;

        public TestReviewManagementController(ITestReviewService testReviewService, IMapper mapper)
        {
            _testReviewService = testReviewService;
            _mapper = mapper;
        }

        [HttpGet("test/{testId:int}/TestReview")]
        [Authorize]
        public async Task<IActionResult> GetTestReview(int testId)
        {
            var testReviewDto = await _testReviewService.GetTestReviewAsync(testId);

            return Ok(_mapper.Map<TestReviewVm>(testReviewDto));
        }

        [HttpPut("test/{testId:int}/TestReview")]
        [Authorize]
        public async Task<IActionResult> GetTestReview(int testId, [FromBody]TestReviewVm testReview)
        {
            var testReviewDto = _mapper.Map<TestReviewDto>(testReview);
            await _testReviewService.UpdateTestReviewAsync(testReviewDto);

            return Ok();
        }
    }
}
