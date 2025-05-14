using ReportPortal.DAL.Enums;

namespace ReportPortal.ViewModels.UserManagement
{
    public class UserVm
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public UserRole UserRole { get; set; }
    }
}
