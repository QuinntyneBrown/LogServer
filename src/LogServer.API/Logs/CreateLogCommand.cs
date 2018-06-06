using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogServer.API
{
    public class CreateLogCommand
    {
        public class Validator: AbstractValidator<Request> {
            public Validator()
            {
                RuleFor(request => request.LogLevel).NotNull();
                RuleFor(request => request.Message).NotEmpty().NotNull();
                RuleFor(request => request.ClientId).NotEqual(default(Guid));
            }
        }

        public class Request : IRequest<Response> {
            public string LogLevel { get; set; }
            public string Message { get; set; }
            public Guid ClientId { get; set; }
        }

        public class Response
        {			
            public int LogId { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public IAppDbContext _context { get; set; }
            public Handler(IAppDbContext context) => _context = context;

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var log = new Log
                {
                    LogLevel = request.LogLevel,
                    Message = request.Message,
                    ClientId = request.ClientId
                };
                _context.Logs.Add(log);
                await _context.SaveChangesAsync(cancellationToken);
                return new Response() { LogId = log.LogId };
            }
        }
    }
}
