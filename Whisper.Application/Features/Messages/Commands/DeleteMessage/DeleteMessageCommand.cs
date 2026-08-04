using FluentValidation;
using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.DeleteMessage;

public record DeleteMessageCommand(Guid MessageId, Guid UserId) : IRequest<Result<Unit>>;

public class DeleteMessageCommandValidator : AbstractValidator<DeleteMessageCommand>
{
    public DeleteMessageCommandValidator()
    {
        RuleFor(x => x.MessageId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
    }
}
