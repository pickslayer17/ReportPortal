using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
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

        public RunManagementController(IRunService runService, IMapper mapper, IFolderService folderService)
        {
            _runService = runService;
            _mapper = mapper;
            _folderService = folderService;
        }

        [HttpPost("AddRun")]
        [Authorize]
        public async Task<IActionResult> AddRun([FromBody] RunCreateVm runVm, CancellationToken cancellationToken = default)
        {
            var runDto = _mapper.Map<RunDto>(runVm);
            RunCreatedDto runCreatedDto = null;
            try
            {
                runCreatedDto = await _runService.CreateAsync(runDto, cancellationToken);
            }
            catch (ProjectNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(_mapper.Map<RunVm>(runCreatedDto));
        }

        [HttpGet("Runs/{runId:int}")]
        [Authorize]
        public async Task<IActionResult> GetRun(int runId, CancellationToken cancellationToken = default)
        {
            var run = await _runService.GetByIdAsync(runId, cancellationToken);

            return Ok(_mapper.Map<RunVm>(run));
        }

        [HttpGet("Project/{projectId:int}/Runs")]
        [Authorize]
        public async Task<IActionResult> GetAllRuns(int projectId, CancellationToken cancellationToken = default)
        {
            var allRunsDto = await _runService.GetAllByAsync(r => r.ProjectId == projectId, cancellationToken);
            var resultVms = allRunsDto.Select(rdto => _mapper.Map<RunVm>(rdto));

            return Ok(resultVms);
        }


        [HttpPost("Runs/{runId:int}/delete")]
        [Authorize]
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
    }
}
