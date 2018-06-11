using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API.Logs
{
    public class SendLogCreatedNotification : INotificationHandler<LogCreatedEvent>
    {
        private readonly IHubContext<AppHub> _hubContext;

        public SendLogCreatedNotification(IHubContext<AppHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(LogCreatedEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("message", notification, cancellationToken);
        }
    }
}
