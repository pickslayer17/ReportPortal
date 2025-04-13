using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using ReportPortal.BL.Services.Interfaces;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Hubs
{
    public class RunUpdatesHub : Hub
    {
        private readonly ITestService _testService;
        private readonly IMapper _mapper;

        public RunUpdatesHub(ITestService testService, IMapper mapper)
        {
            _testService = testService;
            _mapper = mapper;
        }

        public async Task JoinRunGroup(string runId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, runId);
        }
    }
}
