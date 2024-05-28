using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Models.ForCreation;
using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IFolderService : IServiceBase<FolderRunItem, FolderCreatedDto, FolderForCreationDto>
    {
        public Task<int> AddOrGetId(int runId, string path);
    }

}
