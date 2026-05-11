using FluentValidation;

namespace LogServer.Application.Logs.Queries.GetLogs;

public sealed class GetLogsValidator : AbstractValidator<GetLogsQuery>
{
    public GetLogsValidator()
    {
    }
}
