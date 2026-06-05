using AutoMapper;
using Models.Dto;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using ReportPortal.Interfaces;

namespace ReportPortal.BL.Services
{
    public class SubprojectService : ISubprojectService
    {
        private readonly ISubprojectRepository _subprojectRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectScopeRepository _scope;
        private readonly IMapper _mapper;

        public SubprojectService(ISubprojectRepository subprojectRepository, IProjectRepository projectRepository,
            IUserRepository userRepository, IProjectScopeRepository scope, IMapper mapper)
        {
            _subprojectRepository = subprojectRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _scope = scope;
            _mapper = mapper;
        }

        public async Task<SubprojectDto> CreateAsync(int projectId, string name, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(p => p.Id == projectId, cancellationToken);
            if (project == null) throw new ProjectNotFoundException($"Project {projectId} cannot be found.");

            var subproject = new Subproject { Name = name, ProjectId = projectId };
            var id = await _subprojectRepository.InsertAsync(subproject, cancellationToken);

            return new SubprojectDto { Id = id, Name = name, ProjectId = projectId };
        }

        public async Task<IEnumerable<SubprojectDto>> GetForProjectAsync(int projectId, CancellationToken cancellationToken = default)
        {
            var subprojects = await _subprojectRepository.GetForProjectAsync(projectId, cancellationToken);
            return subprojects.Select(s => _mapper.Map<SubprojectDto>(s));
        }

        public async Task AddMemberAsync(int subprojectId, int userId, CancellationToken cancellationToken = default)
        {
            var subproject = await _subprojectRepository.GetByIdAsync(subprojectId, cancellationToken);
            if (subproject == null) throw new SubprojectNotFoundException($"Subproject {subprojectId} cannot be found.");

            // Throws UserNotFoundException if the user is missing.
            await _userRepository.GetByAsync(u => u.Id == userId, cancellationToken);

            // A subproject member must first belong to the parent project (access is project-level).
            if (!await _scope.IsMemberAsync(userId, subproject.ProjectId, cancellationToken))
                throw new ForbiddenAccessException("User must be a member of the parent project first.");

            await _subprojectRepository.AddMemberAsync(userId, subprojectId, cancellationToken);
        }

        public async Task RemoveMemberAsync(int subprojectId, int userId, CancellationToken cancellationToken = default)
        {
            await _subprojectRepository.RemoveMemberAsync(userId, subprojectId, cancellationToken);
        }

        public async Task<IEnumerable<UserDto>> GetMembersAsync(int subprojectId, CancellationToken cancellationToken = default)
        {
            var subproject = await _subprojectRepository.GetByIdAsync(subprojectId, cancellationToken);
            if (subproject == null) throw new SubprojectNotFoundException($"Subproject {subprojectId} cannot be found.");

            var members = await _subprojectRepository.GetMembersAsync(subprojectId, cancellationToken);
            return members.Select(u => _mapper.Map<UserDto>(u));
        }
    }
}
