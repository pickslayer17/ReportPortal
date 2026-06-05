using System.ComponentModel.DataAnnotations;

namespace ReportPortal.ViewModels.UserManagement
{
    public class ChangePasswordVm
    {
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; }
    }
}
