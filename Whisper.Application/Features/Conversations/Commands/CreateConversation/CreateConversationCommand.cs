using FluentValidation;
using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Conversations.Commands.CreateConversation;

public record CreateConversationCommand(Guid CurrentUserId, Guid OtherUserId) : IRequest<Result<Guid>>;

public class CreateConversationCommandValidator : AbstractValidator<CreateConversationCommand>
{
    public CreateConversationCommandValidator()
    {
        RuleFor(x => x.CurrentUserId).NotEmpty();
        RuleFor(x => x.OtherUserId).NotEmpty();
        RuleFor(x => x.OtherUserId).NotEqual(x => x.CurrentUserId).WithMessage("Cannot create a conversation with yourself.");
    }
}
