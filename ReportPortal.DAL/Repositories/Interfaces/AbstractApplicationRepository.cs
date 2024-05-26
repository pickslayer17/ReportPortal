namespace ReportPortal.DAL.Repositories.Interfaces
{
    public abstract class AbstractApplicationRepository
    {
        protected readonly ApplicationContext _dbContext;

        public AbstractApplicationRepository(ApplicationContext dbContext) => _dbContext = dbContext;
    }
}
