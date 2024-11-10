using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.ViewModels.TestRun
{
    public class TestVm
    {
        public int Id { get; set; }
        public int FolderId { get; set; }
        public string Path { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public virtual TestReviewVm TestReview { get; set; }
        public List<TestResultVm> TestResults { get; set; }
    }
}
