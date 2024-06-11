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
        public async Task<IActionResult> AddTestResult(int testId, [FromBody] TestResultVm testVm)
        {
            var testResultDto = _mapper.Map<TestResultDto>(testVm);
            var testResultCreated = await _testResultService.AddTestResultToTestAsync(testId, testResultDto);

            return Ok(testResultCreated);
        }

        [HttpGet("test/{testId:int}/GetTestResultIds")]
        [Authorize]
        public async Task<IActionResult> GetTestResultIds(int testId)
        {
            var testResultIds = await _testResultService.GetTestTestResultIdsAsync(testId);

            return Ok(testResultIds);
        }


        [HttpGet("TestResult/{testResultId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTestResult(int testResultId)
        {
            var testResultDto = await _testResultService.GetByIdAsync(testResultId);

            return Ok(_mapper.Map<TestResultVm>(testResultDto));
        }
    }
}
