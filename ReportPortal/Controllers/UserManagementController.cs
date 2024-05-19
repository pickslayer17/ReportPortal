using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Services.Interfaces;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        public UserManagementController(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        [HttpGet("GetUsers")]
        //[Authorize]
        public IActionResult GetUsers()
        {
            return Ok();
        }

        [HttpPost("CreateUser")]
        //[Authorize(Roles = "Admin")]
        public IActionResult CreateUser(UserForCreationDto userModel)
        {
            _userService.CreateAsync(userModel);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserDto login)
        {
            IActionResult response = null;// Unauthorized();
            var user = _authenticationService.AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _authenticationService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }


    }
}
