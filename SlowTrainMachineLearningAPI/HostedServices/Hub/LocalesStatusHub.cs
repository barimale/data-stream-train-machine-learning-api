using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace Albergue.Administrator.HostedServices.Hub
{
    [Authorize]
    public class LocalesStatusHub : Microsoft.AspNetCore.SignalR.Hub<ILocalesStatusHub>
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine(Context.ConnectionId + " is connected");

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine(Context.ConnectionId + " is disconnected");

            return base.OnDisconnectedAsync(exception);
        }

        public Task OnStartAsync(string id)
        {
            return Clients?.All?.OnStartAsync(id);
        }

        public Task OnFinishAsync(string id)
        {
            return Clients?.All?.OnFinishAsync(id);
        }
    }
}
