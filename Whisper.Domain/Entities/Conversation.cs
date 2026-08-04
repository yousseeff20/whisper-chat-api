using System;
using System.Collections.Generic;

namespace Whisper.Domain.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public string? Title { get; set; }
    public bool IsGroup { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
}
