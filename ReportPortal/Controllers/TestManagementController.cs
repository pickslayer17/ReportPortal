using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Hubs;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestManagementController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;
        private readonly IHubContext<RunUpdatesHub> _hubContext;

        public TestManagementController(ITestService testService, IFolderService folderService, IMapper mapper, IHubContext<RunUpdatesHub> hubContext)
        {
            _testService = testService;
            _folderService = folderService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpPost("AddTest")]
        [Authorize]
        public async Task<IActionResult> AddTest([FromBody] TestSaveVm testVm)
        {
            var testDto = _mapper.Map<TestDto>(testVm);
            var folderId = await _folderService.GetIdOrAddFolderInRun(testVm.RunId, testVm.Path);
            var testCreated = await _testService.CreateAsync(testDto, folderId);

            var testDtoForHub = await _testService.GetByIdAsync(testCreated.Id);
            await _hubContext.Clients.Group(testVm.RunId.ToString()).SendAsync("AddTest", _mapper.Map<TestVm>(testDtoForHub));

            return Ok(testCreated);
        }

        [HttpPost("getTestId")]
        public async Task<IActionResult> GetTestId([FromBody] TestSaveVm model)
        {
            var testFolderId = await _folderService.DoesFolderExists(model.RunId, model.Path);
            if (testFolderId == null) return NotFound();

            var testId = await _testService.GetTestId(testFolderId, model.Name);
            return Ok(testId);
        }

        [HttpGet("tests/{testId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTestById( int testId)
        {
            var test = await _testService.GetByIdAsync(testId);
            return Ok(_mapper.Map<TestVm>(test));
        }

        [HttpGet("Runs/{runId:int}/tests")]
        [Authorize]
        public async Task<IActionResult> GetAllRunTests(int runId)
        {
            var tests = await _testService.GetAllByRunIdAsync(runId);
            return Ok(tests.Select(t => _mapper.Map<TestVm>(t)));
        }

        [HttpGet("Folder/{folderId:int}/tests")]
        [Authorize]
        public async Task<IActionResult> GetAllFolderTests(int folderId)
        {
            var tests = await _testService.GetAllByFolderIdAsync(folderId);
            return Ok(tests.Select(t => _mapper.Map<TestVm>(t)));
        }

        [HttpPost("Test/{testId:int}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteTest(int testId)
        {
            try
            {
                await _testService.DeleteByIdAsync(testId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
