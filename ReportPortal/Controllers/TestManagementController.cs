using AutoMapper;
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
    public class TestManagementController : ControllerBase
    {
        private readonly ITestService _testService;
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public TestManagementController(ITestService testService, IProjectService projectService, IFolderService folderService, IMapper mapper)
        {
            _testService = testService;
            _folderService = folderService;
            _mapper = mapper;
        }

        [HttpPost("AddTest")]
        [Authorize]
        public async Task<IActionResult> AddTest([FromBody] TestVm testVm)
        {
            var testDto = _mapper.Map<TestDto>(testVm);
            var testCreated = await _testService.CreateAsync(testDto);

            return Ok(testCreated);
        }
    }
}
