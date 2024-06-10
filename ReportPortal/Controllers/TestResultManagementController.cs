using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestResultManagementController : ControllerBase
    {
        private readonly ITestResultService _testResultService;
        private readonly IMapper _mapper;

        public TestResultManagementController(ITestResultService testResultService, IMapper mapper)
        {
            _testResultService = testResultService;
            _mapper = mapper;
        }

        [HttpPost("test/{testId:int}/AddTestResult")]
        [Authorize]
        public async Task<IActionResult> AddTest(int testId, [FromBody] TestResultVm testVm)
        {
            var testResultDto = _mapper.Map<TestResultDto>(testVm);
            var testResultCreated = await _testResultService.AddTestResultToTest(testId, testResultDto);

            return Ok(testResultCreated);
        }
    }
}
