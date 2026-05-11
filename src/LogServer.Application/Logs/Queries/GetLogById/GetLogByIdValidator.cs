using FluentValidation;

namespace LogServer.Application.Logs.Queries.GetLogById;

public sealed class GetLogByIdValidator : AbstractValidator<GetLogByIdQuery>
{
    public GetLogByIdValidator()
    {
        RuleFor(x => x.LogId).NotEqual(Guid.Empty);
    }
}
