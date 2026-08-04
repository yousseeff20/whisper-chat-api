using MediatR;
using System;
using Whisper.Domain.Common;

using Whisper.Domain.Enums;

namespace Whisper.Application.Features.Messages.Commands.SendMessage;

public record SendMessageCommand(
    string Content, 
    string ConversationId, 
    string SenderId,
    Guid? ReplyToMessageId = null,
    MessageType MessageType = MessageType.Text,
    string? FileUrl = null,
    string? ImageUrl = null,
    string? ThumbnailUrl = null,
    string? FileName = null,
    long? FileSize = null,
    string? MimeType = null
) : IRequest<Result<SendMessageResponse>>;

public record SendMessageResponse(string MessageId, string Content, string Time, bool IsSentByMe);
