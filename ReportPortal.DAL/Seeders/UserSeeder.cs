using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.Seeders
{
    public class UserSeeder : AbstractSeeder
    {
        public UserSeeder(ApplicationContext context) : base(context)
        {
        }

        public override async Task SeedAsync()
        {
            if (!await _context.Users.AnyAsync(u => u.Email == "d"))
            {
                var user = new User
                {
                    Email = "d",
                    Password = "uZM5dclrT6HDUzQnkBjS/+IJ94Ct6/4JR3KW7pAZtkO4qqGV",
                    UserRole = UserRole.Administrator
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
