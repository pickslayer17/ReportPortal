using System.ComponentModel.DataAnnotations;

namespace ReportPortal.ViewModels.TestRun
{
    public class SubprojectCreateVm
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
