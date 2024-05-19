using Models.Dto;
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

            if (userByEmailResult != null) return Task.Run(() => new UserDto());



            return Task.Run(() => new UserDto());
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
