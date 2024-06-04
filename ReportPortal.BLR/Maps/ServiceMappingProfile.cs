using AutoMapper;
using Models.Dto;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Models;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.BL.Maps
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            CreateMap<TestRunItem, TestDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<TestRunItem, TestCreatedDto>();
            CreateMap<Run, RunDto>().ReverseMap();
            CreateMap<FolderDto, FolderRunItem>().ReverseMap();
        }
    }
}
