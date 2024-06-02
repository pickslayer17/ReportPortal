using Models.Dto;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.UserManagement;
using ReportPortal.Interfaces;
using ReportPortal.Services.Interfaces;
using System.Linq.Expressions;

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

        public async Task<UserCreatedDto> CreateAsync(UserDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            var userByEmailResult = await _userRepository.GetByAsync(u => u.Email == userForCreationDto.Email);

            if (userByEmailResult != null) throw new Exception("User already exists!");

            var userDbModel = new User();
            userDbModel.Email = userForCreationDto.Email;
            userDbModel.Password = _authenticationService.HashPassword(userForCreationDto.Password);
            userDbModel.UserRole = userForCreationDto.Role;

            var createdUserId = await _userRepository.InsertAsync(userDbModel);

            var userCreated = new UserCreatedDto
            {
                Email = userDbModel.Email,
                Id = createdUserId
            };

            return await Task.Run(() => userCreated);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByAsync(u => u.Id == id);
            if(user != null)
            {
                await _userRepository.RemoveAsync(user);
            }
            else
            {
                throw new UserNotFoundException($"User with userId {id} isn't present.");
            }
            
        }

        public Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllByAsync(Expression<Func<UserDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByAsync(Expression<Func<UserDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
