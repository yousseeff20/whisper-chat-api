using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;
using Whisper.Domain.Entities;
using Whisper.Domain.Enums;

namespace Whisper.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<SendMessageResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRealtimeNotifier _notifier;

    public SendMessageCommandHandler(IApplicationDbContext context, IRealtimeNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task<Result<SendMessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.ConversationId, out var conversationId) || 
            !Guid.TryParse(request.SenderId, out var senderId))
        {
            return Result.Failure<SendMessageResponse>(new Error("Message.InvalidId", "Invalid GUID format"));
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversationId,
            SenderId = senderId,
            Text = request.Content,
            MessageType = request.MessageType,
            ReplyToMessageId = request.ReplyToMessageId,
            SentAt = DateTimeOffset.UtcNow,
            IsDeletedForEveryone = false,
            FileUrl = request.FileUrl,
            ImageUrl = request.ImageUrl,
            ThumbnailUrl = request.ThumbnailUrl,
            FileName = request.FileName,
            FileSize = request.FileSize,
            MimeType = request.MimeType
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new SendMessageResponse(
            message.Id.ToString(),
            message.Text ?? string.Empty,
            message.SentAt.ToString("HH:mm"),
            true
        );

        await _notifier.NotifyMessageReceivedAsync(request.ConversationId, response);

        return Result.Success(response);
    }
}
