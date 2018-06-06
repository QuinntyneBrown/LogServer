using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading;
using System.Threading.Tasks;

namespace LogService
{
    public class EntityChangedBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>

    {
        private readonly IHubContext<AppHub> _hubContext;
        private readonly IAppDbContext _context;

        public EntityChangedBehavior(IHubContext<AppHub> hubContext, IAppDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            if (request is CreateLogCommand.Request) {
                await _hubContext.Clients.All.SendAsync("message", new
                {
                    Type = "[Log] Created",
                    Payload = new { log = LogApiModel.FromLog((await _context.Logs.FindAsync((response as CreateLogCommand.Response).LogId))) }
                });
            }

            return response;             
        }
    }
}