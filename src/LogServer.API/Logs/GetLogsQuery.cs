using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class GetLogsQuery
    {
        public class Request : IRequest<Response> { }

        public class Response
        {
            public IEnumerable<LogApiModel> Logs { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public IAppDbContext _context { get; set; }
            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new Response()
                {
                    Logs = await _context.Logs.Select(x => LogApiModel.FromLog(x)).ToListAsync()
                };
        }
    }
}
