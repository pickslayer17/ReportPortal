using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Models.Created;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.DAL.Exceptions;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RunManagementController : ControllerBase
    {
        private readonly IRunService _runService;
        private readonly IMapper _mapper;

        public RunManagementController(IRunService runService, IMapper mapper)
        {
            _runService = runService;
            _mapper = mapper;
        }

        [HttpPost("AddRun")]
        [Authorize]
        public async Task<IActionResult> AddRun([FromBody] RunVm runVm)
        {
            var runDto = _mapper.Map<RunDto>(runVm);
            RunCreatedDto runCreatedDto = null;
            try
            {
                runCreatedDto = await _runService.CreateAsync(runDto);
            }
            catch (ProjectNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(runCreatedDto);
        }

        [HttpGet("Project/{projectId:int}/Runs/{runId:int}")]
        [Authorize]
        public async Task<IActionResult> GetRun(int projectId, int runId)
        {
            var run = await _runService.GetByIdAsync(runId);

            return Ok(_mapper.Map<RunVm>(run));
        }

        [HttpGet("Project/{projectId:int}/Runs}")]
        [Authorize]
        public async Task<IActionResult> GetAllRuns(int projectId)
        {
            var allRunsDto = await _runService.GetAllByAsync(r => r.ProjectId == projectId);
            var resultVms = allRunsDto.Select(rdto => _mapper.Map<RunVm>(rdto));

            return Ok(resultVms);
        }


        [HttpPost("Project/{projectId:int}/Runs/{runId:int}/delete")]
        [Authorize]
        public async Task<IActionResult> DeleteRun(int runId)
        {
            await _runService.DeleteByIdAsync(runId);

            return Ok();
        }
    }
}
