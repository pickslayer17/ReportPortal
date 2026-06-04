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
                    // PBKDF2-SHA256 hash of password "admin" (matches AuthenticationService).
                    // TODO (users epic): seed via the live hashing function instead of a literal,
                    // so it never drifts from the algorithm again.
                    Password = "Bn8t377tgXGbZ4BOPLAcoylA/gIF3ixobn4cdMBt09RBBEZaJcZkuTOSmT1CQGzA",
                    UserRole = UserRole.Administrator
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
