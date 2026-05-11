namespace LogServer.Core.DomainEvents;

public sealed class LogCreated : DomainEvent
{
    public LogCreated(Guid logId, string logLevel, string message, Guid clientId)
    {
        LogId = logId;
        LogLevel = logLevel;
        Message = message;
        ClientId = clientId;
    }

    public Guid LogId { get; }
    public string LogLevel { get; }
    public string Message { get; }
    public Guid ClientId { get; }
}
