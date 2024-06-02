using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

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
            var allProjectsDto = await _projectService.GetAllAsync();
            var allProjectsVm = allProjectsDto.Select(pr => _autoMapperInnerService.Map<ProjectDto, ProjectVm>(pr));
            return Ok(allProjectsVm);
        }

        [HttpGet("GetProject")]
        //[Authorize]
        public async Task<IActionResult> GetAProject(int projectId)
        {
            var projectDto = await _projectService.GetByAsync(pr => pr.Id == projectId);
            var projectVm = _autoMapperInnerService.Map<ProjectDto, ProjectVm>(projectDto);
            return Ok(projectVm);
        }

        [HttpPost("AddProject")]
        //[Authorize]
        public async Task<IActionResult> AddProject([FromBody] ProjectVm projectForCreationDto)
        {
            var projectDto = _autoMapperInnerService.Map<ProjectVm, ProjectDto>(projectForCreationDto);
            return Ok(await _projectService.CreateAsync(projectDto));
        }
    }
}
