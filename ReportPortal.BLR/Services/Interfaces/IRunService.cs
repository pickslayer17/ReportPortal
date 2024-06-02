using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IRunService : IServiceBase<RunDto, RunCreatedDto>
    {
    }
}
