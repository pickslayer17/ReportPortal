using Models.Dto;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models;
using ReportPortal.Interfaces;
using ReportPortal.Services.Interfaces;

namespace ReportPortal.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) => _userRepository = userRepository;

        public Task<UserDto> CreateAsync(UserForCreationDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var userByEmailResult = _userRepository.GetByEmailAsync(userForCreationDto.Email);

            /// verify if user could be inserted into DB
            if (userByEmailResult.Result != null) return Task.Run(() => new UserDto());

            var userDbModel = new User();
            userDbModel.Email = userForCreationDto.Email;
            userDbModel.PasswordSalt = ""; /// generate Salt
            userDbModel.Password = ""; /// encrypt password using salt
            userDbModel.UserRole = UserRole.User;
            _userRepository.InsertAsync(userDbModel);

            userByEmailResult = _userRepository.GetByEmailAsync(userForCreationDto.Email);

            var userCreated = new UserDto
            {
                Email = userDbModel.Email,
                Id = userByEmailResult.Result.Id.Value
            };

            return Task.Run(() => userCreated);
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
