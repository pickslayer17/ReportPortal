using System.ComponentModel.DataAnnotations;

namespace ReportPortal.DAL
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string? Name {  get; set; }
        public string? Password { get; set; }
        public UserRole UserRole { get; set; }
    }
}
