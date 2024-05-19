using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models;

namespace ReportPortal.DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}
