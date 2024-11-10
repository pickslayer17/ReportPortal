using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services;
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
        public async Task<IActionResult> GetAllFolders(int runId)
        {
            var allFoldersDto = await _folderService.GetAllFolders(runId);
            var allProjectsVm = allFoldersDto.Select(f => _mapper.Map<FolderVm>(f));
            return Ok(allProjectsVm);
        }

        [HttpDelete("folder/{folderId:int}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteFolder(int folderId)
        {
            var folder = await _folderService.GetByIdAsync(folderId);
            if (folder == null)  
                throw new Exception($"Folder not found with id {folderId}"); 

            var runId = folder.RunId;
            var removedFolderVm = _mapper.Map<FolderVm>(folder);

            await _folderService.DeleteFolder(folderId);

            await _hubContext.Clients.Group(runId.ToString()).SendAsync("RemoveFolder", removedFolderVm);

            return Ok();
        }
    }
}
