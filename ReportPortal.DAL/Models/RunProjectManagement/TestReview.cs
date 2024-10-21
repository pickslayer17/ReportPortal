using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class TestReview
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public virtual Test Test { get; set; }
        public int ReviewerId { get; set; }
        public virtual User Reviewer { get; set; }
        public string? Comments { get; set; }
        public TestReviewOutcome TestReviewOutcome { get; set; }
    }
}
