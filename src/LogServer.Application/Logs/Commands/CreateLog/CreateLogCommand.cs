using MediatR;

namespace LogServer.Application.Logs.Commands.CreateLog;

public sealed class CreateLogCommand : IRequest<CreateLogResponse>
{
    public string LogLevel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Guid ClientId { get; set; }
}
