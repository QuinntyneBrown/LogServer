using LogServer.Application.Common.Interfaces;
using LogServer.Core.Logs;
using MediatR;

namespace LogServer.Application.Logs.Commands.CreateLog;

public sealed class CreateLogHandler : IRequestHandler<CreateLogCommand, CreateLogResponse>
{
    private readonly ILogServerDbContext _context;
    private readonly IPublisher _publisher;

    public CreateLogHandler(ILogServerDbContext context, IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<CreateLogResponse> Handle(CreateLogCommand request, CancellationToken cancellationToken)
    {
        var log = new Log(request.LogLevel, request.Message, request.ClientId);

        _context.Logs.Add(log);

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in log.DomainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        log.ClearDomainEvents();

        return new CreateLogResponse { LogId = log.LogId };
    }
}
