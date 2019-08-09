using LogServer.Core.Interfaces;
using LogServer.Core.Models;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class GetLogById
    {
        public class Request : IRequest<Response> {
            public Guid LogId { get; set; }
        }

        public class Response
        {
            public LogDto Log { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IEventStore _eventStore;

            public Handler(IEventStore eventStore) => _eventStore = eventStore;
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new Response() {
                    Log = LogDto.FromLog(_eventStore.Query<Log>(request.LogId))
                };
        }
    }
}
