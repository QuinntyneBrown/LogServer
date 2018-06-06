using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LogService
{
    public class AppHub: Hub
    {
        public async Task Send(string message)
            => await Clients.All.SendAsync("message", message);
    }
}
