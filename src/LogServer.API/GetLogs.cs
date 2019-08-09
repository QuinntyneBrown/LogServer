using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class GetLogs
    {
        public class Request : IRequest<Response> { }

        public class Response
        {
            public ICollection<LogDto> Logs { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventStore _eventStore;

            public Handler(IEventStore eventStore) => _eventStore = eventStore;
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new Response()
                {
                    Logs = _eventStore.Query<Log>().Select(x => LogDto.FromLog(x)).ToList()
                };
        }
    }
}
