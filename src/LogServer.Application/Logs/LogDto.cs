using LogServer.Core.Logs;

namespace LogServer.Application.Logs;

public sealed class LogDto
{
    public Guid LogId { get; init; }
    public string LogLevel { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public Guid ClientId { get; init; }
    public DateTime CreatedOn { get; init; }

    public static LogDto FromLog(Log log) => new()
    {
        LogId = log.LogId,
        LogLevel = log.LogLevel,
        Message = log.Message,
        ClientId = log.ClientId,
        CreatedOn = log.CreatedOn,
    };
}
