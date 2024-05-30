using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Constants;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.ForCreation;

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

        [HttpGet("GetUsers")]
        [Authorize]
        public IActionResult GetUsers()
        {
            return Ok();
        }

        [HttpPost("CreateUser")]
        //[Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> CreateUser([FromBody] UserForCreationDto userModel)
        {
            var userDto = _autoMapperInnerService.Map<UserForCreationDto, UserDto>(userModel);

            var userCreated = await _userService.CreateAsync(userDto);

            return Ok(userCreated);
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
