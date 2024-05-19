using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.Services.Interfaces;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private IUserService _userService;

        public UserManagementController(IUserService userService) => _userService = userService;

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
    }
}
