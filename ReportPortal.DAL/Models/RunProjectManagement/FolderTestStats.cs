namespace ReportPortal.DAL.Models.RunProjectManagement
{
    /// <summary>
    /// Per-folder test counters (direct tests of the folder). Subtree roll-up is done
    /// by the caller over the folder tree — cheap, since there are far fewer folders
    /// than tests, and the frontend never needs to load the 15k test models.
    /// </summary>
    public class FolderTestStats
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
