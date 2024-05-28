using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models.ForCreation;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IProjectService _projectService;
        private readonly IFolderService _folderService;

        public TestController(ITestService testService, IProjectService projectService, IFolderService folderService)
        {
            _testService = testService;
            _projectService = projectService;
            _folderService = folderService;
        }

        [HttpGet("GetAllTests")]
        //[Authorize]
        public async Task<IActionResult> GetAllTests(int projectId)
        {
            return Ok(await _testService.GetAllAsync());
        }


        [HttpPost("AddTest")]
        //[Authorize]
        public async Task<IActionResult> AddTest([FromBody]TestForCreationDto testForCreationDto)
        {
            var project = await _projectService.GetByAsync(pr => pr.Id == testForCreationDto.RunId);
            if (project == null)
            {
                return BadRequest(new { Message =  "There is no project with such id!" });
            }

            var folderId = await _folderService.GetIdOrAddFolderInRun(testForCreationDto.RunId, testForCreationDto.Path);
            var test = new TestRunItem
            {
                Name = testForCreationDto.Name,
                ParentId = folderId,
            };
            //_testService.CreateAsync()

            return Ok(await _testService.GetAllAsync());
        }
    }
}
