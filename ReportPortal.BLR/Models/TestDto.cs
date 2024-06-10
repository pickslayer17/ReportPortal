namespace ReportPortal.BL.Models
{
    public class TestDto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public int FolderId { get; set; }
        public List<int>? TestResultIds { get; set; }
    }

}
