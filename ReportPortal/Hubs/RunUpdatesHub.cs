using Microsoft.AspNetCore.SignalR;
using ReportPortal.ViewModels.TestRun;

namespace ReportPortal.Hubs
{
    public class RunUpdatesHub : Hub
    {
        public async Task SendFolderUpdate(FolderVm[] folders)
        {
            await Clients.All.SendAsync("UpdateFolders", folders);
        }

        public async Task SendTestUpdate(TestVm[] tests)
        {
            await Clients.All.SendAsync("UpdateTests", tests);
        }
    }
}
