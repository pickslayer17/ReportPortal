using AutoMapper;
using Models.Dto;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.ForCreation;

namespace ReportPortal.Services
{
    public class AutoMapperInnerService : IAutoMapperInnerService
    {
        IMapper _mapper;
        public AutoMapperInnerService()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserForCreationDto, UserDto>());
            _mapper = config.CreateMapper();
        }

        public T Map<K, T>(K entity) where T : class
        {
            return _mapper.Map<T>(entity);
        }
    }
}
