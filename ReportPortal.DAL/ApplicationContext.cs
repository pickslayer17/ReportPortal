using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models;
using ReportPortal.DAL.Models.TestResult;

namespace ReportPortal.DAL
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<FolderRunItem> Folders { get; set; }
        public DbSet<TestRunItem> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}
