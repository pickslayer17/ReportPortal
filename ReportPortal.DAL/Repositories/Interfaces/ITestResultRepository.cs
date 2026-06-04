using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface ITestResultRepository : IRepository<TestResult>
    {
        Task InsertRangeAsync(IEnumerable<TestResult> testResults, CancellationToken cancellationToken = default);
    }
}
