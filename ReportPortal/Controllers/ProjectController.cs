using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models.ForCreation;
using ReportPortal.BL.Services.Interfaces;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("GetAllProject")]
        //[Authorize]
        public async Task<IActionResult> GetAllProjects()
        {
            return Ok(await _projectService.GetAllAsync());
        }

        [HttpPost("AddProject")]
        //[Authorize]
        public async Task<IActionResult> AddProject([FromBody] ProjectForCreationDto projectForCreationDto)
        {
            return Ok(await _projectService.CreateAsync(projectForCreationDto));
        }
    }
}
