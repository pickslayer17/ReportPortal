namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Test
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public int FolderId { get; set; }
        public virtual Folder Folder { get; set; }
        public virtual ICollection<TestResult> TestResults { get; set; }
        public virtual TestReview TestReview { get; set; }
    }
}
