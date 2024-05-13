using Models.Dto;

namespace ReportPortal.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllByOwnerIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserDto> GetByIdAsync(Guid userId, Guid accountId, CancellationToken cancellationToken);
        Task<UserDto> CreateAsync(Guid userId, UserForCreationDto accountForCreationDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid userId, Guid accountId, CancellationToken cancellationToken = default);
    }
}
