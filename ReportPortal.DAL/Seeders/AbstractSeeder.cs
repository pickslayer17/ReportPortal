namespace ReportPortal.DAL.Seeders
{
    public abstract class AbstractSeeder
    {
        protected readonly ApplicationContext _context;

        public AbstractSeeder(ApplicationContext context)
        {
            _context = context;
        }

        public abstract Task SeedAsync();
    }
}
