using AutoMapper;
using Models.Dto;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.UserManagement;

namespace ReportPortal.Services
{
    public class AutoMapperInnerService : IAutoMapperInnerService
    {
        IMapper _mapper;
        public AutoMapperInnerService()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserVm, UserDto>());
            _mapper = config.CreateMapper();
        }

        public T Map<K, T>(K entity) where T : class
        {
            return _mapper.Map<T>(entity);
        }
    }
}
