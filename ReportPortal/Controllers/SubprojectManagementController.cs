using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.Authorization;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;
using ReportPortal.ViewModels.UserManagement;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubprojectManagementController : ControllerBase
    {
        private readonly ISubprojectService _subprojectService;
        private readonly IMapper _mapper;

        public SubprojectManagementController(ISubprojectService subprojectService, IMapper mapper)
        {
            _subprojectService = subprojectService;
            _mapper = mapper;
        }

        // projectId in route -> ProjectScopeFilter ensures the caller is a project member (or admin).
        [HttpPost("Project/{projectId:int}/subprojects")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> Create(int projectId, [FromBody] SubprojectCreateVm model, CancellationToken cancellationToken = default)
        {
            var created = await _subprojectService.CreateAsync(projectId, model.Name, cancellationToken);
            return Ok(_mapper.Map<SubprojectVm>(created));
        }

        [HttpGet("Project/{projectId:int}/subprojects")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetForProject(int projectId, CancellationToken cancellationToken = default)
        {
            var subprojects = await _subprojectService.GetForProjectAsync(projectId, cancellationToken);
            return Ok(subprojects.Select(s => _mapper.Map<SubprojectVm>(s)));
        }

        [HttpGet("Subproject/{subprojectId:int}/members")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetMembers(int subprojectId, CancellationToken cancellationToken = default)
        {
            var members = await _subprojectService.GetMembersAsync(subprojectId, cancellationToken);
            return Ok(members.Select(u => _mapper.Map<UserVm>(u)));
        }

        [HttpPost("Subproject/{subprojectId:int}/members/{userId:int}")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> AddMember(int subprojectId, int userId, CancellationToken cancellationToken = default)
        {
            await _subprojectService.AddMemberAsync(subprojectId, userId, cancellationToken);
            return Ok();
        }

        [HttpDelete("Subproject/{subprojectId:int}/members/{userId:int}")]
        [Authorize(Policy = Permissions.ManageProjects)]
        public async Task<IActionResult> RemoveMember(int subprojectId, int userId, CancellationToken cancellationToken = default)
        {
            await _subprojectService.RemoveMemberAsync(subprojectId, userId, cancellationToken);
            return Ok();
        }
    }
}
