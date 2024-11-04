using ReportPortal.DAL.Enums;

namespace ReportPortal.BL.Models
{
    public class TestReviewUpdateDto
    {
        public int Id { get; set; }
        public Optional<int?> ReviewerId { get; set; } = Optional<int?>.None();
        public Optional<string?> Comments { get; set; } = Optional<string?>.None();
        public Optional<TestReviewOutcome> TestReviewOutcome { get; set; } = Optional<TestReviewOutcome>.None();
    }
}
