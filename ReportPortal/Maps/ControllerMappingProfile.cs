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
            CreateMap<RunVm, RunDto>().ReverseMap(); ;
            CreateMap<ProjectDto, ProjectVm>().ReverseMap(); ;
            CreateMap<UserDto, UserVm>().ForMember(dest => dest.Password, opt => opt.Condition(src => false));
            CreateMap<UserVm, UserDto>().ReverseMap(); ;
            CreateMap<ProjectVm, ProjectDto>().ReverseMap();
        }
    }
}
