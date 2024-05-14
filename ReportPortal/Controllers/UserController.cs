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

        public UserController(IUserService userService) => _userService = userService;

        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return new List<User> { new User { Id = 1, Name = "asd"} };
        }

        [HttpPost]
        public int CreateUser(User userModel)
        {
            // get guid
            var guid = new Guid();

            //create UserForCreationModelDto
            var userForCreationDto = new UserForCreationDto();
            //map fields from userModel

            var result = _userService.CreateAsync(guid, userForCreationDto);
            var createdUserId = result.Result.Id;

            return 0;
        }
    }
}
