using LogServer.Core.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class GetLogByIdQuery
    {
        public class Request : IRequest<Response> {

        }

        public class Response
        {

        }

        public class Handler : IRequestHandler<Request, Response>
        {

            private readonly IEventStore _eventStore;

            public Handler(IEventStore eventStore) => _eventStore = eventStore;
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                return new Response() { };
            }
        }
    }
}
