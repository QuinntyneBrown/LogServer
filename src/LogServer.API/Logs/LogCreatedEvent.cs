using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API.Logs
{
    public class LogCreatedEvent
    {
        public class Notification : INotification
        {
            public Notification(Log log)
                => Payload = new { log = LogApiModel.FromLog(log) };

            public string Type { get; set; } = "[Log] Created";
            public dynamic Payload { get; set; }
        }

        public class Handler : INotificationHandler<Notification>
        {
            private readonly IHubContext<AppHub> _hubContext;

            public Handler(IHubContext<AppHub> hubContext)
                => _hubContext = hubContext;

            public async Task Handle(Notification notification, CancellationToken cancellationToken)
                => await _hubContext.Clients.All.SendAsync("message", notification, cancellationToken);
        }
    }
}