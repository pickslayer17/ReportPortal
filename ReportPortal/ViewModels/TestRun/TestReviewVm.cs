﻿using ReportPortal.DAL.Enums;

namespace ReportPortal.ViewModels.TestRun
{
    public class TestReviewVm
    {
        public int Id { get; set; }
        public int? ReviewerId { get; set; }
        public string? Comments { get; set; }
        public TestReviewOutcome TestReviewOutcome { get; set; }
        public int? ProductBug { get; set; }
    }
}
