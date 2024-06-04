using AutoMapper;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.BL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IRunService _runService;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IMapper mapper, IRunService runService)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
            _runService = runService;
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

        public async Task DeleteByIdAsync(int id)
        {
            var existingProject = await _projectRepository.GetByAsync(pr => pr.Id == id);
            if (existingProject != null)
            {
                throw new ProjectNotFoundException($"There is no project with such id {id}");
            }

            await _projectRepository.RemoveAsync(existingProject);

            // delete all runs of the project
            var projectRuns = await _runService.GetAllByAsync(pr => pr.ProjectId == id);
            foreach ( var run in projectRuns) 
            {
                await _runService.DeleteByIdAsync(run.Id);
            }
        }

        public async Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var project = await _projectRepository.GetByAsync(pr => pr.Id == id);

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var allProjects = await _projectRepository.GetAllByAsync(pr => true);
            var allProjectsDto = allProjects.Select(pr => _mapper.Map<ProjectDto>(pr));

            return allProjectsDto;
        }

        public Task<IEnumerable<ProjectDto>> GetAllByAsync(Expression<Func<ProjectDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectDto> GetByAsync(Expression<Func<ProjectDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
