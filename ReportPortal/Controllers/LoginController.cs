using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public LoginController(IJwtService jwtService) => _jwtService = jwtService;


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserDto login)
        {
            IActionResult response = null;// Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _jwtService.GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }



        private UserDto AuthenticateUser(UserDto login)
        {
            UserDto user = null;

            if (true)//login.Email == "Jignesh")
            {
                user = new UserDto { Email = "test.btest@gmail.com" };
            }

            return user;
        }
    }
}
