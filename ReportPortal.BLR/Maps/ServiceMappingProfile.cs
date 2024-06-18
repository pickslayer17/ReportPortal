﻿using AutoMapper;
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
            CreateMap<Test, TestDto>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Test, TestCreatedDto>();
            CreateMap<Run, RunDto>().ReverseMap();
            CreateMap<FolderDto, Folder>().ReverseMap();
            CreateMap<TestResult, TestResultDto>().ReverseMap();
        }
    }
}
