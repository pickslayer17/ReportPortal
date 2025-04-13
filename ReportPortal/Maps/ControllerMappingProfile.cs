using AutoMapper;
using Models.Dto;
using ReportPortal.BL.Models;
using ReportPortal.ViewModels.TestRun;
using ReportPortal.ViewModels.UserManagement;

namespace ReportPortal.Maps
{
    public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
            CreateMap<TestVm, TestDto>().ReverseMap();
            CreateMap<RunVm, RunDto>().ReverseMap();
            CreateMap<RunCreateVm, RunDto>().ReverseMap();
            CreateMap<ProjectDto, ProjectVm>().ReverseMap();
            CreateMap<UserVm, UserDto>().ReverseMap();
            CreateMap<UserLoginVm, UserDto>().ReverseMap();
            CreateMap<UserCreateVm, UserDto>().ReverseMap();
            CreateMap<ProjectVm, ProjectDto>().ReverseMap();
            CreateMap<ProjectCreateVm, ProjectDto>().ReverseMap();
            CreateMap<FolderVm, FolderDto>().ReverseMap();
            CreateMap<TestResultVm, TestResultDto>().ReverseMap();
            CreateMap<TestResultCreateVm, TestResultDto>().ReverseMap();
            CreateMap<TestReviewVm, TestReviewDto>().ReverseMap();
            CreateMap<TestSaveVm, TestDto>().ReverseMap();
        }
    }
}
