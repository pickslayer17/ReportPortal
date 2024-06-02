using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportPortal.BL.Models;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RunController : ControllerBase
    {
        private readonly IRunService _runService;
        private readonly IAutoMapperInnerService _autoMapperInnerService;

        public RunController(IRunService runService, IAutoMapperInnerService autoMapperInnerService)
        {
            _runService = runService;
            _autoMapperInnerService = autoMapperInnerService;
        }


        [HttpPost("AddRun")]
        [Authorize]
        public async Task<IActionResult> AddRun([FromBody] RunVm runVm)
        {
            var runDto = _autoMapperInnerService.Map<RunVm, RunDto>(runVm);
            return Ok(await _runService.CreateAsync(runDto));
        }
    }
}
