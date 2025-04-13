using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Hubs;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FolderManagementController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;
        private readonly IHubContext<RunUpdatesHub> _hubContext;

        public FolderManagementController(IFolderService folderService, IMapper mapper, IHubContext<RunUpdatesHub> hubContext)
        {
            _folderService = folderService;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        [HttpGet("Runs/{runId:int}/folders")]
        [Authorize]
        public async Task<IActionResult> GetAllFolders(int runId, CancellationToken cancellationToken = default)
        {
            var allFoldersDto = await _folderService.GetAllFoldersAsync(runId, cancellationToken);
            var allProjectsVm = allFoldersDto.Select(f => _mapper.Map<FolderVm>(f));
            return Ok(allProjectsVm);
        }

        [HttpDelete("folder/{folderId:int}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteFolder(int folderId, CancellationToken cancellationToken = default)
        {
            var folder = await _folderService.GetByIdAsync(folderId, cancellationToken);
            if (folder == null)
                throw new Exception($"Folder not found with id {folderId}");

            var runId = folder.RunId;
            var removedFolderVm = _mapper.Map<FolderVm>(folder);

            await _folderService.DeleteFolderAsync(folderId, cancellationToken);

            await _hubContext.Clients.Group(runId.ToString()).SendAsync("RemoveFolder", removedFolderVm, cancellationToken);

            return Ok();
        }
    }
}
