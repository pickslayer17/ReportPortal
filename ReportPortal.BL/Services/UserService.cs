using AutoMapper;
using Models.Dto;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.UserManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using ReportPortal.Interfaces;
using ReportPortal.Services.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IProjectRepository projectRepository, IAuthenticationService authenticationService, IMapper mapper)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _authenticationService = authenticationService;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(UserDto userForCreationDto, CancellationToken cancellationToken = default)
        {
            // verify if user already exists
            User userByEmailResult = null;
            try
            {
                userByEmailResult = await _userRepository.GetByAsync(u => u.Email == userForCreationDto.Email, cancellationToken);
            }
            catch (UserNotFoundException) { }
            if (userByEmailResult != null) throw new EmailAlreadyExistsException("Email is already in use.");

            var userDbModel = new User();
            userDbModel.Email = userForCreationDto.Email;
            userDbModel.Password = _authenticationService.HashPassword(userForCreationDto.Password);
            userDbModel.UserRole = userForCreationDto.UserRole;

            var createdUserId = await _userRepository.InsertAsync(userDbModel, cancellationToken);

            var userCreated = new UserDto
            {
                Email = userDbModel.Email,
                Id = createdUserId
            };

            return userCreated;
        }

        public async Task DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            await _userRepository.RemoveByIdAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllByAsync(u => true, cancellationToken);
            var usersDto = users.Select(u => _mapper.Map<UserDto>(u));

            return usersDto;
        }

        public async Task<IEnumerable<UserDto>> GetColleaguesAsync(int userId, CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetUsersSharingProjectsAsync(userId, cancellationToken);
            return users.Select(u => _mapper.Map<UserDto>(u));
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByAsync(u => u.Id == userId, cancellationToken);
            if (!_authenticationService.VerifyHash(user.Password, currentPassword))
                return false;

            user.Password = _authenticationService.HashPassword(newPassword);
            await _userRepository.UpdateItemAsync(user, cancellationToken);
            return true;
        }

        public async Task<UserDto> UpdateProfileAsync(int userId, string email, CancellationToken cancellationToken = default)
        {
            User userWithSameEmail = null;
            try
            {
                userWithSameEmail = await _userRepository.GetByAsync(u => u.Email == email, cancellationToken);
            }
            catch (UserNotFoundException) { }
            if (userWithSameEmail != null && userWithSameEmail.Id != userId)
                throw new EmailAlreadyExistsException("Email is already in use.");

            var user = await _userRepository.GetByAsync(u => u.Id == userId, cancellationToken);
            user.Email = email;
            await _userRepository.UpdateItemAsync(user, cancellationToken);

            return _mapper.Map<UserDto>(user);
        }

        public async Task AddMemberAsync(int projectId, int userId, CancellationToken cancellationToken = default)
        {
            // Throws UserNotFoundException if the user is missing.
            await _userRepository.GetByAsync(u => u.Id == userId, cancellationToken);

            var project = await _projectRepository.GetByAsync(p => p.Id == projectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException($"Project {projectId} cannot be found.");

            await _userRepository.AddUserToProjectAsync(userId, projectId, cancellationToken);
        }

        public async Task RemoveMemberAsync(int projectId, int userId, CancellationToken cancellationToken = default)
        {
            await _userRepository.RemoveUserFromProjectAsync(userId, projectId, cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetProjectMembersAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(p => p.Id == projectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException($"Project {projectId} cannot be found.");

            var members = await _userRepository.GetProjectMembersAsync(projectId, cancellationToken);
            return members.Select(u => _mapper.Map<UserDto>(u));
        }

        public Task<IEnumerable<UserDto>> GetAllByAsync(Expression<Func<UserDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetByAsync(Expression<Func<UserDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByAsync(u => u.Id == id, cancellationToken);
            return _mapper.Map<UserDto>(user);
        }
    }
}
