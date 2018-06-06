using MediatR;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LogService
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
