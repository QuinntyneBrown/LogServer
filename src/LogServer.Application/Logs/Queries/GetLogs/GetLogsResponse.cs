namespace LogServer.Application.Logs.Queries.GetLogs;

public sealed class GetLogsResponse
{
    public IReadOnlyCollection<LogDto> Logs { get; init; } = Array.Empty<LogDto>();
}
