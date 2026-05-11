using LogServer.Core.Common;
using LogServer.Core.DomainEvents;

namespace LogServer.Core.Logs;

public class Log : AggregateRoot
{
    // EF Core constructor.
    private Log()
    {
        LogLevel = string.Empty;
        Message = string.Empty;
    }

    public Log(string logLevel, string message, Guid clientId)
    {
        LogId = Guid.NewGuid();
        LogLevel = logLevel;
        Message = message;
        ClientId = clientId;
        CreatedOn = DateTime.UtcNow;

        RaiseDomainEvent(new LogCreated(LogId, LogLevel, Message, ClientId));
    }

    public Guid LogId { get; private set; }
    public string LogLevel { get; private set; }
    public string Message { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime CreatedOn { get; private set; }
}
