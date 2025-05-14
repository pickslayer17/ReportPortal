using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
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
        public async Task<IActionResult> AddTest([FromBody] TestSaveVm testVm, CancellationToken cancellationToken = default)
        {
            var testDto = _mapper.Map<TestDto>(testVm);
            var folderId = await _folderService.GetIdOrAddFolderInRunAsync(testVm.RunId, testVm.Path, cancellationToken);
            var testCreated = await _testService.CreateAsync(testDto, folderId, cancellationToken);

            var testDtoForHub = await _testService.GetByIdAsync(testCreated.Id, cancellationToken);
            await _hubContext.Clients.Group(testVm.RunId.ToString()).SendAsync("AddTest", _mapper.Map<TestVm>(testDtoForHub), cancellationToken);

            return Ok(_mapper.Map<TestVm>(testCreated));
        }

        [HttpPost("getTestId")]
        public async Task<IActionResult> GetTestId([FromBody] TestSaveVm model, CancellationToken cancellationToken = default)
        {
            int testFolderId;
            try
            {
                testFolderId = await _folderService.DoesFolderExistsAsync(model.RunId, model.Path, cancellationToken);
            }
            catch (FolderNotFoundException)
            {
                return Ok(-1);
            }

            var testId = await _testService.GetTestIdAsync(testFolderId, model.Name, cancellationToken);
            return Ok(testId);
        }

        [HttpDelete("tests/{testId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteTestById(int testId, CancellationToken cancellationToken = default)
        {
            var test = await _testService.GetByIdAsync(testId, cancellationToken);
            if (test == null) return NotFound();

            var runId = test.RunId;
            await _testService.DeleteByIdAsync(testId, cancellationToken);

            await _hubContext.Clients.Group(runId.ToString()).SendAsync("RemoveTest", testId, cancellationToken);

            return Ok();
        }

        [HttpGet("tests/{testId:int}")]
        [Authorize]
        public async Task<IActionResult> GetTestById(int testId, CancellationToken cancellationToken = default)
        {
            var test = await _testService.GetByIdAsync(testId, cancellationToken);
            return Ok(_mapper.Map<TestVm>(test));
        }

        [HttpGet("Runs/{runId:int}/tests")]
        [Authorize]
        public async Task<IActionResult> GetAllRunTests(int runId, CancellationToken cancellationToken = default)
        {
            var tests = await _testService.GetAllByRunIdAsync(runId, cancellationToken);
            return Ok(tests.Select(t => _mapper.Map<TestVm>(t)));
        }

        [HttpGet("Folder/{folderId:int}/tests")]
        [Authorize]
        public async Task<IActionResult> GetAllFolderTests(int folderId, CancellationToken cancellationToken = default)
        {
            var tests = await _testService.GetAllByFolderIdAsync(folderId, cancellationToken);
            return Ok(tests.Select(t => _mapper.Map<TestVm>(t)));
        }

        [HttpPost("Test/{testId:int}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteTest(int testId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _testService.DeleteByIdAsync(testId, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
