using FluentValidation;
using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.EditMessage;

public record EditMessageCommand(Guid MessageId, string NewText, Guid UserId) : IRequest<Result<Unit>>;

public class EditMessageCommandValidator : AbstractValidator<EditMessageCommand>
{
    public EditMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewText).NotEmpty().MaximumLength(4000);
    }
}
