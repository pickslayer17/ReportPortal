using Models.Dto;
using ReportPortal.DAL;
using ReportPortal.Interfaces;

namespace ReportPortal.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) => _userRepository = userRepository;
        public Task<UserDto> CreateAsync(Guid userId, UserForCreationDto accountForCreationDto, CancellationToken cancellationToken = default)
        {
            // creat user model from accountForCreationDto
            var userModel = new User();

            _userRepository.Insert(userModel);

            //create dto model for return
            var userDto = new UserDto();

            return Task.FromResult(userDto);
        }

        public Task DeleteAsync(Guid userId, Guid accountId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllByOwnerIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByIdAsync(Guid userId, Guid accountId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
