using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.ForCreation;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IAutoMapperInnerService _autoMapperInnerService;

        public ProjectController(IProjectService projectService, IAutoMapperInnerService autoMapperInnerService)
        {
            _projectService = projectService;
            _autoMapperInnerService = autoMapperInnerService;
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
            var projectDto = _autoMapperInnerService.Map<ProjectForCreationDto, ProjectDto>(projectForCreationDto);
            return Ok(await _projectService.CreateAsync(projectDto));
        }
    }
}
