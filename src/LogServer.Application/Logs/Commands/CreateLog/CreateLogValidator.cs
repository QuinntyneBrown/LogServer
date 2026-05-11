using FluentValidation;

namespace LogServer.Application.Logs.Commands.CreateLog;

public sealed class CreateLogValidator : AbstractValidator<CreateLogCommand>
{
    public CreateLogValidator()
    {
        RuleFor(x => x.LogLevel)
            .NotEmpty()
            .MaximumLength(32);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.ClientId)
            .NotEqual(Guid.Empty);
    }
}
