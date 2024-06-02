﻿using AutoMapper;
using Models.Dto;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.BL.Services
{
    public class AutoMapperService : IAutoMapperService
    {
        IMapper _mapper;
        public AutoMapperService()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TestDto, TestRunItem>();
                cfg.CreateMap<TestRunItem, TestCreatedDto>();
                cfg.CreateMap<Project, ProjectDto>();
                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<User, UserDto>();
            });

            _mapper = config.CreateMapper();
        }

        public T Map<K, T>(K entity) where T : class
        {
            return _mapper.Map<T>(entity);
        }
    }
}
