using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ITestService : IServiceBase<TestDto, TestCreatedDto>
    {
    }
}
