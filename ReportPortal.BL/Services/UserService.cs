﻿using AutoMapper;
using Models.Dto;
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
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IAuthenticationService authenticationService, IMapper mapper)
        {
            _userRepository = userRepository;
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
            if (userByEmailResult != null) throw new Exception("User already exists!");

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
