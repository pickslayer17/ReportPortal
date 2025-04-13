using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
using ReportPortal.DAL.Exceptions;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectManagementController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        public ProjectManagementController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet("GetAllProject")]
        [Authorize]
        public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken = default)
        {
            var allProjectsDto = await _projectService.GetAllAsync(cancellationToken);
            var allProjectsVm = allProjectsDto.Select(pr => _mapper.Map<ProjectVm>(pr));
            return Ok(allProjectsVm);
        }

        [HttpGet("GetProject/{projectId:int}")]
        [Authorize]
        public async Task<IActionResult> GetProject(int projectId, CancellationToken cancellationToken = default)
        {
            var projectDto = await _projectService.GetByIdAsync(projectId, cancellationToken);
            var projectVm = _mapper.Map<ProjectDto>(projectDto);
            return Ok(projectVm);
        }

        [HttpPost("AddProject")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> AddProject([FromBody] ProjectCreateVm projectForCreationVm, CancellationToken cancellationToken = default)
        {
            var projectDto = _mapper.Map<ProjectDto>(projectForCreationVm);
            return Ok(_mapper.Map<ProjectVm>( _projectService.CreateAsync(projectDto, cancellationToken).Result));
        }

        [HttpPost("DeleteProject/{projectId:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteProject(int projectId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _projectService.DeleteByIdAsync(projectId, cancellationToken);
            }
            catch (ProjectNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
