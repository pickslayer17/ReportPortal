using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Hubs;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestResultManagementController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly ITestResultService _testResultService;
        private readonly IMapper _mapper;
        private readonly IHubContext<RunUpdatesHub> _hubContext;

        public TestResultManagementController(ITestResultService testResultService, IMapper mapper, IHubContext<RunUpdatesHub> hubContext, ITestService testService)
        {
            _testResultService = testResultService;
            _mapper = mapper;
            _hubContext = hubContext;
            _testService = testService;
        }

        [HttpPost("test/{testId:int}/AddTestResult")]
        [Authorize]
        public async Task<IActionResult> AddTestResult(int testId, [FromBody] TestResultVm testVm)
        {
            var testResultDto = _mapper.Map<TestResultDto>(testVm);
            var testResultCreated = await _testResultService.AddTestResultToTestAsync(testId, testResultDto);

            var testDtoForHub = await _testService.GetByIdAsync(testId);
            await _hubContext.Clients.Group(testDtoForHub.RunId.ToString()).SendAsync("UpdateTestResult", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(testResultCreated);
        }

        [HttpGet("test/{testId:int}/TestResults")]
        [Authorize]
        public async Task<IActionResult> GetTestResultIds(int testId)
        {
            var testResult = await _testResultService.GetTestTestResultsAsync(testId);

            return Ok(testResult.Select(tr => _mapper.Map<TestResultVm>(tr)));
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
