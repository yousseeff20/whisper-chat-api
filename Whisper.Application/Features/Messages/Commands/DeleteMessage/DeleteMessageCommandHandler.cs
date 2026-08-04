using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Commands.DeleteMessage;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result<Unit>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRealtimeNotifier _notifier;

    public DeleteMessageCommandHandler(IApplicationDbContext context, IRealtimeNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<Result<Unit>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.Messages
            .FirstOrDefaultAsync(m => m.Id == request.MessageId, cancellationToken);

        if (message == null)
            return Result<Unit>.Failure<Unit>(new Error("Message.NotFound", "Message not found."));

        if (message.SenderId != request.UserId)
            return Result<Unit>.Failure<Unit>(new Error("Message.Unauthorized", "Not authorized to delete this message."));

        if (message.IsDeletedForEveryone)
            return Result<Unit>.Success(Unit.Value);

        message.IsDeletedForEveryone = true;
        message.DeletedAt = DateTimeOffset.UtcNow;
        message.Text = "This message was deleted.";
        message.ImageUrl = null;
        message.FileUrl = null;
        message.StoragePath = null;
        message.FileName = null;
        message.ThumbnailUrl = null;

        await _context.SaveChangesAsync(cancellationToken);

        // Notify via SignalR
        await _notifier.NotifyMessageDeletedAsync(message.ConversationId, message.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
