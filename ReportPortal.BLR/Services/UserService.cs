using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models;
using ReportPortal.Interfaces;
using ReportPortal.Services.Interfaces;

namespace ReportPortal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthenticationService _authenticationService;

        public UserService(IUserRepository userRepository, IAuthenticationService authenticationService)
        {
            _userRepository = userRepository;
            _authenticationService = authenticationService;
        }

        public async Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var userByEmailResult = await _userRepository.GetByAsync(u => u.Email == userForCreationDto.Email);

            if (userByEmailResult != null) throw new Exception("User already exists!");

            var userDbModel = new User();
            userDbModel.Email = userForCreationDto.Email;
            userDbModel.Password = _authenticationService.HashPassword(userForCreationDto.Password);
            userDbModel.UserRole = UserRole.User;

            var createdUserId = await _userRepository.InsertAsync(userDbModel);

            var userCreated = new UserDto
            {
                Email = userDbModel.Email,
                Id = createdUserId
            };

            return await Task.Run(() => userCreated);
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
