using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class FolderManagementController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public FolderManagementController(IFolderService folderService, IMapper mapper)
        {
            _folderService = folderService;
            _mapper = mapper;
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
            await _folderService.DeleteFolder(folderId);
            return Ok();
        }
    }
}
