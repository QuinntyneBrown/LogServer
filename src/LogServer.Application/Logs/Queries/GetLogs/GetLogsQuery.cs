using MediatR;

namespace LogServer.Application.Logs.Queries.GetLogs;

public sealed class GetLogsQuery : IRequest<GetLogsResponse>
{
}
