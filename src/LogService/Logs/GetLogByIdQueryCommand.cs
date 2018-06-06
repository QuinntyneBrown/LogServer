using MediatR;
using System.Threading.Tasks;
using System.Threading;
using FluentValidation;

namespace LogService
{
    public class GetLogByIdQuery
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(request => request.LogId).NotEqual(0);
            }
        }

        public class Request : IRequest<Response> {
            public int LogId { get; set; }
        }

        public class Response
        {
            public LogApiModel Log { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public IAppDbContext _context { get; set; }
            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
                => new Response()
                {
                    Log = LogApiModel.FromLog(await _context.Logs.FindAsync(request.LogId))
                };
        }
    }
}
