using Microsoft.AspNetCore.Mvc;
using Models.Dto;
using ReportPortal.DAL;
using ReportPortal.Interfaces;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        [HttpGet(Name = "GetUsers")]
        public IEnumerable<User> Get()
        {
            return new List<User> { new User { Id = 1, Name = "asd"} };
        }

        [HttpPost(Name = "PostUser")]
        public int Post(User userModel)
        {
            // get guid
            var guid = new Guid();

            //create UserForCreationModelDto
            var userForCreationDto = new UserForCreationDto();
            //map fields from userModel

            var result = _userService.CreateAsync(guid, userForCreationDto);
            var createdUserId = result.Result.Id;

            return createdUserId;
        }
    }
}
