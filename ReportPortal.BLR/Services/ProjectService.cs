using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IAutoMapperService _autoMapperService;

        public ProjectService(IProjectRepository projectRepository, IAutoMapperService autoMapperService)
        {
            _projectRepository = projectRepository;
            _autoMapperService = autoMapperService;
        }

        public async Task<ProjectCreatedDto> CreateAsync(ProjectDto projectForCreationDto, CancellationToken cancellationToken = default)
        {
            var existingProject = await _projectRepository.GetByAsync(pr => pr.Name == projectForCreationDto.Name);

            if (existingProject != null)
            {
                return new ProjectCreatedDto { IsCreated = false };
            }
            else
            {
                var projectToAdd = new Project
                {
                    Name = projectForCreationDto.Name,
                    ProjectStatus = ProjectStatus.Started
                };
                var projectId = await _projectRepository.InsertAsync(projectToAdd);

                return new ProjectCreatedDto { IsCreated = true, Id = projectId };
            }
        }

        public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allProjects = await _projectRepository.GetAllByAsync(pr => true);
            var allProjectsDto = allProjects.Select(pr => _autoMapperService.Map<Project, ProjectDto>(pr));

            return allProjectsDto;
        }

        public Task<IEnumerable<ProjectDto>> GetAllByAsync(Expression<Func<ProjectDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectDto> GetByAsync(Expression<Func<ProjectDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //var project = await _projectRepository.GetByAsync(predicate);

            //return _autoMapperService.Map<Project, ProjectDto>(project);
        }
    }
}
