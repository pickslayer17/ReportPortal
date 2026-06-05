using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.Authorization;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Exceptions;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.UserManagement;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public UserManagementController(IUserService userService, IAuthenticationService authenticationService, IMapper mapper)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        [HttpGet("GetUser/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int userId, CancellationToken cancellationToken = default)
        {
            var userDto = await _userService.GetByIdAsync(userId, cancellationToken);
            var userVm = _mapper.Map<UserVm>(userDto);
            return Ok(userVm);
        }

        // Contextual user list (e.g. reviewer dropdown). Non-admins only see colleagues
        // they share a project with; admins see everyone.
        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
        {
            IEnumerable<UserDto> usersDto;
            if (User.IsInRole(UserRoles.Admin))
            {
                usersDto = await _userService.GetAllAsync(cancellationToken);
            }
            else
            {
                var currentUserId = CurrentUserId();
                if (currentUserId == null) return Unauthorized();
                usersDto = await _userService.GetColleaguesAsync(currentUserId.Value, cancellationToken);
            }

            var usersVm = usersDto.Select(u => _mapper.Map<UserVm>(u));
            return Ok(usersVm);
        }

        // Admin-only: every user in the system, regardless of project membership.
        [HttpGet("GetAllProjectsUsers")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> GetAllProjectsUsers(CancellationToken cancellationToken = default)
        {
            var usersDto = await _userService.GetAllAsync(cancellationToken);
            var usersVm = usersDto.Select(u => _mapper.Map<UserVm>(u));

            return Ok(usersVm);
        }

        [AllowAnonymous]
        [HttpPost("SetupAdmin")]
        public async Task<IActionResult> SetupAdmin([FromBody] UserCreateVm model, CancellationToken cancellationToken = default)
        {
            // First-run bootstrap: allowed only while there are no users yet.
            var anyUserExists = (await _userService.GetAllAsync(cancellationToken)).Any();
            if (anyUserExists)
                return Conflict(new { message = "Setup already completed: an administrator already exists." });

            var userDto = _mapper.Map<UserDto>(model);
            userDto.UserRole = UserRole.Administrator; // force admin regardless of the payload

            var createdAdmin = await _userService.CreateAsync(userDto, cancellationToken);
            return Ok(_mapper.Map<UserVm>(createdAdmin));
        }

        [HttpPost("CreateUser")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateVm userModel, CancellationToken cancellationToken = default)
        {
            var userDto = _mapper.Map<UserDto>(userModel);

            var userCreated = await _userService.CreateAsync(userDto, cancellationToken);
            return Ok(_mapper.Map<UserVm>(userCreated));
        }

        [HttpPost("DeleteUser/{userId:int}")]
        [Authorize(Policy = Permissions.ManageUsers)]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken = default)
        {
            await _userService.DeleteByIdAsync(userId, cancellationToken);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginVm userLoginVm, CancellationToken cancellationToken = default)
        {
            IActionResult response = Unauthorized();
            var userLoginDto = _mapper.Map<UserDto>(userLoginVm);
            var user = await _authenticationService.AuthenticateUserAsync(userLoginDto, cancellationToken);

            if (user != null)
            {
                var tokenString = _authenticationService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        [HttpGet("ValidateToken")]
        [Authorize]
        public async Task<IActionResult> ValidateToken(CancellationToken cancellationToken = default)
        {
            return Ok();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var userDto = await _userService.GetByIdAsync(currentUserId.Value, cancellationToken);
            return Ok(_mapper.Map<UserVm>(userDto));
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileVm model, CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var userDto = await _userService.UpdateProfileAsync(currentUserId.Value, model.Email, cancellationToken);
            return Ok(_mapper.Map<UserVm>(userDto));
        }

        [HttpPost("me/change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordVm model, CancellationToken cancellationToken = default)
        {
            var currentUserId = CurrentUserId();
            if (currentUserId == null) return Unauthorized();

            var changed = await _userService.ChangePasswordAsync(currentUserId.Value, model.CurrentPassword, model.NewPassword, cancellationToken);
            if (!changed) return BadRequest(new { message = "Current password is incorrect." });

            return Ok();
        }

        private int? CurrentUserId()
        {
            return int.TryParse(User.FindFirst("UserId")?.Value, out var id) ? id : null;
        }
    }
}
