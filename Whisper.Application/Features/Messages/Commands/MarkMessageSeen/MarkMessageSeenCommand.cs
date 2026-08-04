using FluentValidation;
using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.MarkMessageSeen;

public record MarkMessageSeenCommand(Guid MessageId, Guid UserId) : IRequest<Result<Unit>>;

public class MarkMessageSeenCommandValidator : AbstractValidator<MarkMessageSeenCommand>
{
    public MarkMessageSeenCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
