using Models.Dto;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.Services.Interfaces
{
    public interface IUserService : IServiceBase<UserDto, UserCreatedDto>
    {
    }
}
