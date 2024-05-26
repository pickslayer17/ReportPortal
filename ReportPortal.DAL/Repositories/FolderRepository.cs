using ReportPortal.DAL.Models;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class FolderRepository : AbstractApplicationRepository, IFolderRepository
    {
        public FolderRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public Task<IEnumerable<FolderRunItem>> GetAllByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<FolderRunItem> GetByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(FolderRunItem folder)
        {
            _dbContext.Folders.Add(folder);
            await _dbContext.SaveChangesAsync();

            return folder.Id;
        }

        public Task RemoveAsync(FolderRunItem user)
        {
            throw new NotImplementedException();
        }
    }
}
