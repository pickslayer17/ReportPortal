using Models.Dto;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Models.ForCreation;
using ReportPortal.BL.Services.Interfaces;

namespace ReportPortal.Services.Interfaces
{
    public interface IUserService : IServiceBase<UserDto, UserCreatedDto, UserForCreationDto>
    {
    }
}
