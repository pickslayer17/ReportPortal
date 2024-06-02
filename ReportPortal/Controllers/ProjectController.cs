using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
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
        [Authorize]
        public async Task<IActionResult> GetAllProjects()
        {
            var allProjectsDto = await _projectService.GetAllAsync();
            var allProjectsVm = allProjectsDto.Select(pr => _autoMapperInnerService.Map<ProjectDto, ProjectVm>(pr));
            return Ok(allProjectsVm);
        }

        [HttpGet("GetProject")]
        [Authorize]
        public async Task<IActionResult> GetProject(int projectId)
        {
            var projectDto = await _projectService.GetByIdAsync(projectId);
            var projectVm = _autoMapperInnerService.Map<ProjectDto, ProjectVm>(projectDto);
            return Ok(projectVm);
        }

        [HttpPost("AddProject")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AddProject([FromBody] ProjectVm projectForCreationDto)
        {
            var projectDto = _autoMapperInnerService.Map<ProjectVm, ProjectDto>(projectForCreationDto);
            return Ok(await _projectService.CreateAsync(projectDto));
        }

        [HttpPost("AddProject")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var projectDto = _autoMapperInnerService.Map<ProjectVm, ProjectDto>(projectForCreationDto);
            return Ok(await _projectService.DeleteAsync(projectId));
        }
    }
}
