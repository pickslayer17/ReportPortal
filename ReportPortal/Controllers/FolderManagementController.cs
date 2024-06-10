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
        public async Task<IActionResult> GetRunChildFolders(int runId)
        {
            var foldersDto = await _folderService.GetRunChildrenAsync(runId);

            return Ok(foldersDto.Select(f => _mapper.Map<FolderVm>(f)));
        }

        [HttpGet("Folder/{folderId:int}")]
        [Authorize]
        public async Task<IActionResult> GetChildFolders(int folderId)
        {
            var foldersDto = await _folderService.GetChildrenAsync(folderId);

            return Ok(foldersDto.Select(f => _mapper.Map<FolderVm>(f)));
        }
    }
}
