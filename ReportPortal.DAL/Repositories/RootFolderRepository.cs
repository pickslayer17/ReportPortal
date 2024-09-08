using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class RootFolderRepository : AbstractApplicationRepository, IRootFolderRepository
    {
        public RootFolderRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<RootFolder>> GetAllByAsync(Expression<Func<RootFolder, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<RootFolder> GetByAsync(Expression<Func<RootFolder, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(RootFolder rootFolder)
        {
            _dbContext.RootFolders.Add(rootFolder);
            await _dbContext.SaveChangesAsync();

            return rootFolder.Id;
        }

        public async Task RemoveByIdAsync(int itemId)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItem(RootFolder item)
        {
            throw new NotImplementedException();
        }
    }
}
