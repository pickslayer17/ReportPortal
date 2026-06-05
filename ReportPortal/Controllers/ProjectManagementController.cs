using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.Authorization;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
using ReportPortal.DAL.Exceptions;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;
using ReportPortal.ViewModels.UserManagement;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectManagementController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProjectManagementController(IProjectService projectService, IUserService userService, IMapper mapper)
        {
            _projectService = projectService;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("GetAllProject")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetAllProjects(CancellationToken cancellationToken = default)
        {
            // Non-admins only see projects they are a member of; admins see everything.
            IEnumerable<ProjectDto> allProjectsDto;
            if (User.IsInRole(UserRoles.Admin))
            {
                allProjectsDto = await _projectService.GetAllAsync(cancellationToken);
            }
            else
            {
                if (!int.TryParse(User.FindFirst("UserId")?.Value, out var userId)) return Unauthorized();
                allProjectsDto = await _projectService.GetForUserAsync(userId, cancellationToken);
            }

            var allProjectsVm = allProjectsDto.Select(pr => _mapper.Map<ProjectVm>(pr));
            return Ok(allProjectsVm);
        }

        [HttpGet("GetProject/{projectId:int}")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetProject(int projectId, CancellationToken cancellationToken = default)
        {
            var projectDto = await _projectService.GetByIdAsync(projectId, cancellationToken);
            var projectVm = _mapper.Map<ProjectDto>(projectDto);
            return Ok(projectVm);
        }

        [HttpPost("AddProject")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> AddProject([FromBody] ProjectCreateVm projectForCreationVm, CancellationToken cancellationToken = default)
        {
            var projectDto = _mapper.Map<ProjectDto>(projectForCreationVm);
            var createdProject = await _projectService.CreateAsync(projectDto, cancellationToken);
            return Ok(_mapper.Map<ProjectVm>(createdProject));
        }

        [HttpPost("DeleteProject/{projectId:int}")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> DeleteProject(int projectId, CancellationToken cancellationToken = default)
        {
            await _projectService.DeleteByIdAsync(projectId, cancellationToken);
            return Ok(new { message = "Проект был удален" });
        }

        // --- Project membership (multitenancy) ---

        [HttpGet("{projectId:int}/members")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetProjectMembers(int projectId, CancellationToken cancellationToken = default)
        {
            var membersDto = await _userService.GetProjectMembersAsync(projectId, cancellationToken);
            return Ok(membersDto.Select(u => _mapper.Map<UserVm>(u)));
        }

        [HttpPost("{projectId:int}/members/{userId:int}")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> AddProjectMember(int projectId, int userId, CancellationToken cancellationToken = default)
        {
            await _userService.AddMemberAsync(projectId, userId, cancellationToken);
            return Ok();
        }

        [HttpDelete("{projectId:int}/members/{userId:int}")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> RemoveProjectMember(int projectId, int userId, CancellationToken cancellationToken = default)
        {
            await _userService.RemoveMemberAsync(projectId, userId, cancellationToken);
            return Ok();
        }
    }
}
