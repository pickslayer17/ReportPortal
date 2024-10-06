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
    public class TestManagementController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public TestManagementController(ITestService testService, IFolderService folderService, IMapper mapper)
        {
            _testService = testService;
            _folderService = folderService;
            _mapper = mapper;
        }

        [HttpPost("AddTest")]
        [Authorize]
        public async Task<IActionResult> AddTest([FromBody] TestVm testVm)
        {
            var testDto = _mapper.Map<TestDto>(testVm);
            var folderId = await _folderService.GetIdOrAddFolderInRun(testVm.RunId, testVm.Path);
            var testCreated = await _testService.CreateAsync(testDto, folderId);

            return Ok(testCreated);
        }

        [HttpGet("Runs/{runId:int}/tests")]
        [Authorize]
        public async Task<IActionResult> GetAllRunTests(int runId)
        {
            var tests = await _testService.GetAllByFolderIdAsync(runId);
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
