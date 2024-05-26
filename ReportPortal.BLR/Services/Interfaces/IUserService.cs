using Models.Dto;
using ReportPortal.BL.Models;

namespace ReportPortal.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<UserCreatedDto> CreateAsync(UserForCreationDto accountForCreationDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
