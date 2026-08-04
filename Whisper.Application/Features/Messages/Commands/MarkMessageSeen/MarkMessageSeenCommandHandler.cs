using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.MarkMessageSeen;

public class MarkMessageSeenCommandHandler : IRequestHandler<MarkMessageSeenCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRealtimeNotifier _notifier;

    public MarkMessageSeenCommandHandler(IApplicationDbContext context, IRealtimeNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<Result<Unit>> Handle(MarkMessageSeenCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.Messages
            .Include(m => m.Conversation)
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);

        if (message == null)
            return Result<Unit>.Failure<Unit>(new Error("Message.NotFound", "Message not found."));

        if (message.SenderId == request.UserId)
            return Result<Unit>.Success(Unit.Value); // Can't see own message

        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == message.ConversationId && cp.UserId == request.UserId, cancellationToken);

        if (!isParticipant)
            return Result<Unit>.Failure<Unit>(new Error("Conversation.Unauthorized", "Not authorized to access this conversation."));

        if (message.IsSeen)
            return Result<Unit>.Success(Unit.Value);

        message.IsSeen = true;

        await _context.SaveChangesAsync(cancellationToken);

        // Notify via SignalR
        await _notifier.NotifyMessageSeenAsync(message.ConversationId, message.Id, request.UserId);

        return Result<Unit>.Success(Unit.Value);
    }
}
