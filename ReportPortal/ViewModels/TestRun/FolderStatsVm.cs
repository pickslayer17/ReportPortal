namespace ReportPortal.ViewModels.TestRun
{
    public class FolderStatsVm
    {
        public int FolderId { get; set; }
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int NotRun { get; set; }
        public int ToInvestigate { get; set; }
        public int NotRepro { get; set; }
        public int ProductBug { get; set; }
    }
}
