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
            var userByEmailResult = _userRepository.GetByEmailAsync(userForCreationDto.Email);

            /// verify if user already exists in DB
            if (userByEmailResult.Result != null) return await Task.Run(() => new UserDto());

            var userDbModel = new User();
            userDbModel.Email = userForCreationDto.Email;
            //userDbModel.PasswordSalt = _authenticationService.GenerateSalt();
            userDbModel.Password = _authenticationService.HashPassword(userForCreationDto.Password);
            userDbModel.UserRole = UserRole.User;
            _userRepository.InsertAsync(userDbModel);

            userByEmailResult = _userRepository.GetByEmailAsync(userForCreationDto.Email);
            await userByEmailResult;

            var userCreated = new UserDto
            {
                Email = userDbModel.Email,
                Id = userByEmailResult.Result.Id.Value
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
