using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
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
        private readonly IAutoMapperInnerService _autoMapperInnerService;

        public UserManagementController(IUserService userService, IAuthenticationService authenticationService, IAutoMapperInnerService autoMapperInnerService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
            _autoMapperInnerService = autoMapperInnerService;
        }

        [HttpGet("GetUser/{userId:int}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int userId)
        {
            var userDto = await _userService.GetByIdAsync(userId);
            var userVm = _autoMapperInnerService.Map<UserDto, UserVm>(userDto);
            return Ok(userVm);
        }

        [HttpGet("GetUsers")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var usersDto = await _userService.GetAllAsync();
            var usersVm = usersDto.Select(u => _autoMapperInnerService.Map<UserDto, UserVm>(u));
            return Ok(usersVm);
        }

        [HttpPost("CreateUser")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] UserVm userModel)
        {
            var userDto = _autoMapperInnerService.Map<UserVm, UserDto>(userModel);

            var userCreated = await _userService.CreateAsync(userDto);

            return Ok(userCreated);
        }

        [HttpPost("DeleteUser/{userId:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteByIdAsync(userId);
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto login)
        {
            IActionResult response = Unauthorized();
            var user = await _authenticationService.AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _authenticationService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }
    }
}
