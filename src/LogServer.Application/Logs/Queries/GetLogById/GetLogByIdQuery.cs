using MediatR;

namespace LogServer.Application.Logs.Queries.GetLogById;

public sealed class GetLogByIdQuery : IRequest<GetLogByIdResponse>
{
    public Guid LogId { get; set; }
}
