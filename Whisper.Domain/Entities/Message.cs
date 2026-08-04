using System;
using Whisper.Domain.Enums;

namespace Whisper.Domain.Entities;

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string? Text { get; set; }
    public MessageType MessageType { get; set; }
    public Guid? ReplyToMessageId { get; set; }
    
    // File/Media Metadata
    public string? FileName { get; set; }
    public long? FileSize { get; set; }
    public string? MimeType { get; set; }
    public string? StoragePath { get; set; }
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? FileUrl { get; set; }
    
    // Status
    public bool IsSeen { get; set; }
    public DateTimeOffset SentAt { get; set; }
    public DateTimeOffset? EditedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public bool IsDeletedForEveryone { get; set; }

    // Navigation Properties
    public Conversation Conversation { get; set; } = null!;
    public User Sender { get; set; } = null!;
    public Message? ReplyToMessage { get; set; }
}
