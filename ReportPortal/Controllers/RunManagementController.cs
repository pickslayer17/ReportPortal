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
        private readonly IAutoMapperInnerService _autoMapperInnerService;

        public RunManagementController(IRunService runService, IAutoMapperInnerService autoMapperInnerService)
        {
            _runService = runService;
            _autoMapperInnerService = autoMapperInnerService;
        }

        [HttpPost("AddRun")]
        [Authorize]
        public async Task<IActionResult> AddRun([FromBody] RunVm runVm)
        {
            var runDto = _autoMapperInnerService.Map<RunVm, RunDto>(runVm);
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

        [HttpPost("DeleteRun")]
        [Authorize]
        public async Task<IActionResult> DeleteRun([FromBody] RunVm runVm)
        {
            

            return Ok();
        }
    }
}
