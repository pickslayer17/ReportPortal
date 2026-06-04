using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
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

        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
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
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateVm userModel, CancellationToken cancellationToken = default)
        {
            var userDto = _mapper.Map<UserDto>(userModel);

            var userCreated = await _userService.CreateAsync(userDto, cancellationToken);

            return Ok(_mapper.Map<UserVm>(userCreated));
        }

        [HttpPost("DeleteUser/{userId:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteUser(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _userService.DeleteByIdAsync(userId, cancellationToken);
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

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
    }
}
