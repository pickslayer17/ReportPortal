using System.ComponentModel.DataAnnotations;
using ReportPortal.DAL.Enums;

namespace ReportPortal.ViewModels.UserManagement
{
    public class UserCreateVm
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public UserRole UserRole { get; set; }
    }
}
