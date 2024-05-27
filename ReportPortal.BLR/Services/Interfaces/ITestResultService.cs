using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Models.ForCreation;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestResultService : IServiceBase<TestResultDto, TestResultCreatedDto, TestResultForCreationDto>
    {
    }
}
