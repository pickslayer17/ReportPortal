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

        public async Task<IEnumerable<Folder>> GetAllByAsync(Expression<Func<Folder, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders
                .Where(predicate)
                .Select(f => new Folder
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentId = f.ParentId,
                    // ... другие нужные поля
                    Tests = f.Tests.Select(t => new Test { Id = t.Id }).ToList()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<Folder> GetByAsync(Expression<Func<Folder, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<int> InsertAsync(Folder folder, CancellationToken cancellationToken = default)
        {
            _dbContext.Folders.Add(folder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return folder.Id;
        }

        public async Task RemoveByIdAsync(int folderId, CancellationToken cancellationToken = default)
        {
            var folder = await GetByAsync(f => f.Id == folderId, cancellationToken);
            _dbContext.Folders.Remove(folder);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Folder> UpdateItemAsync(Folder item, CancellationToken cancellationToken = default)
        {
            var oldItem = await _dbContext.Folders.FirstOrDefaultAsync(f => f.Id == item.Id, cancellationToken);
            _dbContext.Folders.Entry(oldItem).CurrentValues.SetValues(item);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return item;
        }

        public async Task<bool> ExistsAsync(Expression<Func<Folder, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders.AnyAsync(predicate, cancellationToken);
        }

        public async Task<List<Folder>> GetByRunAsync(int runId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Folders
                .AsNoTracking()
                .Where(f => f.RunId == runId)
                .Select(f => new Folder
                {
                    Id = f.Id,
                    Name = f.Name,
                    ParentId = f.ParentId,
                    FolderLevel = f.FolderLevel
                })
                .ToListAsync(cancellationToken);
        }

        public async Task InsertRangeAsync(IEnumerable<Folder> folders, CancellationToken cancellationToken = default)
        {
            await _dbContext.Folders.AddRangeAsync(folders, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
