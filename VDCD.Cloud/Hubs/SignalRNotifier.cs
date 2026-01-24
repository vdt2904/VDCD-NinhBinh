using Microsoft.AspNetCore.SignalR;
using VDCD.Business.Infrastructure;

namespace VDCD.Hubs
{
    public class SignalRNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hub;

        public SignalRNotifier(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public Task Notify(string eventName, object data)
        {
            return _hub.Clients.All.SendAsync(eventName, data);
        }
    }
}
