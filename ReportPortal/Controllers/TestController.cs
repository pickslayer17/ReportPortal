using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IProjectService _projectService;
        private readonly IFolderService _folderService;
        private readonly IAutoMapperInnerService _autoMapperInnerService;

        public TestController(ITestService testService, IProjectService projectService, IFolderService folderService, IAutoMapperInnerService autoMapperInnerService)
        {
            _testService = testService;
            _projectService = projectService;
            _folderService = folderService;
            _autoMapperInnerService = autoMapperInnerService;
        }

        [HttpPost("AddTest")]
        //[Authorize]
        public async Task<IActionResult> AddTest([FromBody] TestVm testVm)
        {
            var project = await _projectService.GetByAsync(pr => pr.Id == testVm.RunId);
            if (project == null)
            {
                return BadRequest(new { Message = "There is no project with such id!" });
            }

            var folderId = await _folderService.GetIdOrAddFolderInRun(testVm.RunId, testVm.Path);
            var testDto = _autoMapperInnerService.Map<TestVm, TestDto>(testVm);
            testDto.FolderId = folderId;

            return Ok(await _testService.CreateAsync(testDto));
        }
    }
}
