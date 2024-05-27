using System.ComponentModel.DataAnnotations;
using ReportPortal.DAL.Enums;

namespace ReportPortal.DAL.Models.UserManagement
{
    public class User
    {
        [Key]
        public int? Id { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserRole UserRole { get; set; }
    }
}
