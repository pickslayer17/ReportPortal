using System.ComponentModel.DataAnnotations;

namespace ReportPortal.ViewModels.UserManagement
{
    public class UpdateProfileVm
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
    }
}
