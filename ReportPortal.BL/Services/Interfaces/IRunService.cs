using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IRunService : IServiceBase<RunDto>
    {
        Task<IEnumerable<RunDto>> GetByProjectAsync(int projectId, CancellationToken cancellationToken = default);
    }
}
