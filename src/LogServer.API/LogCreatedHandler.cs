using LogServer.Core;
using LogServer.Core.DomainEvents;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class LogCreatedHandler : INotificationHandler<LogCreated>
    {
        private readonly IHubContext<IntegrationEventsHub> _hubContext;

        public LogCreatedHandler(IHubContext<IntegrationEventsHub> hubContext)
            => _hubContext = hubContext;
        public async Task Handle(LogCreated notification, CancellationToken cancellationToken)
            => await _hubContext.Clients.All.SendAsync("message", notification, cancellationToken);
    }
}
