using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.Authorization;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.DAL.Repositories.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RunManagementController : ControllerBase
    {
        private readonly IRunService _runService;
        private readonly IMapper _mapper;
        private readonly IFolderService _folderService;
        private readonly ITrxParserService _trxParserService;
        private readonly IProjectScopeRepository _scope;

        public RunManagementController(IRunService runService, IMapper mapper, IFolderService folderService, ITrxParserService trxParserService, IProjectScopeRepository scope)
        {
            _runService = runService;
            _mapper = mapper;
            _folderService = folderService;
            _trxParserService = trxParserService;
            _scope = scope;
        }

        [HttpPost("AddRun")]
        [Authorize(Policy = Permissions.UploadResults)]
        public async Task<IActionResult> AddRun([FromBody] RunCreateVm runVm, CancellationToken cancellationToken = default)
        {
            // Body carries the subprojectId, so the route-based ProjectScopeFilter can't see it: guard here.
            await ScopeGuard.EnsureAccessAsync(User, _scope, ScopeResource.Subproject, runVm.SubprojectId, cancellationToken);

            var runDto = _mapper.Map<RunDto>(runVm);
            var runCreatedDto = await _runService.CreateAsync(runDto, cancellationToken);

            return Ok(_mapper.Map<RunVm>(runCreatedDto));
        }

        [HttpGet("Runs/{runId:int}")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetRun(int runId, CancellationToken cancellationToken = default)
        {
            var run = await _runService.GetByIdAsync(runId, cancellationToken);

            return Ok(_mapper.Map<RunVm>(run));
        }

        [HttpGet("Subproject/{subprojectId:int}/Runs")]
        [Authorize(Policy = Permissions.ViewProjects)]
        public async Task<IActionResult> GetAllRuns(int subprojectId, CancellationToken cancellationToken = default)
        {
            var allRunsDto = await _runService.GetBySubprojectAsync(subprojectId, cancellationToken);
            var resultVms = allRunsDto.Select(rdto => _mapper.Map<RunVm>(rdto));

            return Ok(resultVms);
        }

        [HttpPost("Runs/{runId:int}/delete")]
        [Authorize(Policy = Permissions.DeleteRuns)]
        public async Task<IActionResult> DeleteRun(int runId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _runService.DeleteByIdAsync(runId, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"There were some troubles with run deleting (run.id = {runId})\n{ex.Message}");
            }
        }

        [HttpPost("Subproject/{subprojectId:int}/upload-trx")]
        [Authorize(Policy = Permissions.UploadResults)]
        [RequestSizeLimit(524288000)] // 500 MB, при необходимости увеличьте
        public async Task<IActionResult> UploadTrxFile(int subprojectId, [FromForm] IFormFile file, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран или пустой.");

            if (!file.FileName.EndsWith(".trx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Неверный формат файла. Ожидается .trx файл.");

            // Each uploaded TRX represents one NUnit test run -> create a fresh Run for it.
            var safeFileName = Path.GetFileName(file.FileName);
            var run = await _runService.CreateAsync(
                new RunDto
                {
                    SubprojectId = subprojectId,
                    Name = $"{Path.GetFileNameWithoutExtension(safeFileName)} ({DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC)"
                },
                cancellationToken);

            var uploadsFolder = Path.Combine(Path.GetTempPath(), "trx_uploads");
            Directory.CreateDirectory(uploadsFolder);

            // GetFileName strips any path components to avoid path traversal via FileName.
            var filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{safeFileName}");

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream, cancellationToken);
                }

                await _trxParserService.AddTestsFromXml(filePath, runId: run.Id, cancellationToken: cancellationToken);
            }
            finally
            {
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            return Ok(new { message = "Файл успешно загружен", runId = run.Id });
        }
    }
}
