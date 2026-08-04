using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.EditMessage;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRealtimeNotifier _notifier;

    public EditMessageCommandHandler(IApplicationDbContext context, IRealtimeNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<Result<Unit>> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);

        if (message == null)
            return Result<Unit>.Failure<Unit>(new Error("Message.NotFound", "Message not found."));

        if (message.SenderId != request.UserId)
            return Result<Unit>.Failure<Unit>(new Error("Message.Unauthorized", "Not authorized to edit this message."));

        if (message.IsDeletedForEveryone)
            return Result<Unit>.Failure<Unit>(new Error("Message.Deleted", "Cannot edit a deleted message."));

        message.Text = request.NewText;
        message.EditedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        // Notify via SignalR
        await _notifier.NotifyMessageEditedAsync(message.ConversationId, message.Id, message.Text);

        return Result<Unit>.Success(Unit.Value);
    }
}
