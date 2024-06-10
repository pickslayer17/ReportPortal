namespace ReportPortal.ViewModels.TestRun
{
    public class FolderVm
    {
        public int Id { get; set; }
        public int RunId { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public List<int>? ChildFolderIds { get; set; }
        public List<int>? TestIds { get; set; }
    }
}
