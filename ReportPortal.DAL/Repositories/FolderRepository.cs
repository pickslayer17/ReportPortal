using Microsoft.EntityFrameworkCore;
using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ReportPortal.DAL.Repositories
{
    public class FolderRepository : AbstractApplicationRepository, IFolderRepository
    {
        public FolderRepository(ApplicationContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<FolderRunItem>> GetAllByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders.Where(predicate).ToListAsync();
        }

        public async Task<FolderRunItem> GetByAsync(Expression<Func<FolderRunItem, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> InsertAsync(FolderRunItem folder)
        {
            _dbContext.Folders.Add(folder);
            await _dbContext.SaveChangesAsync();

            return folder.Id;
        }

        public async Task RemoveByIdAsync(int folderId)
        {
            var folder = await GetByAsync(f => f.Id == folderId);
            _dbContext.Folders.Remove(folder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateItem(FolderRunItem item)
        {
            var oldItem = await _dbContext.Folders.FirstOrDefaultAsync(f => f.Id == item.Id);
            _dbContext.Folders.Entry(oldItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync();
        }
    }
}
