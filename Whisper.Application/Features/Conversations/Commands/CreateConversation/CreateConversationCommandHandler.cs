using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;
using Whisper.Domain.Entities;

namespace Whisper.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;

    public CreateConversationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        // Check if the other user exists
        var otherUserExists = await _context.Users.AnyAsync(u => u.Id == request.OtherUserId, cancellationToken);
        if (!otherUserExists)
            return Result<Guid>.Failure<Guid>(new Error("User.NotFound", "Other user not found."));

        // Check if a direct conversation already exists between the two users
        var existingConversation = await _context.Conversations
            .Where(c => !c.IsGroup)
            .Where(c => c.Participants.Any(p => p.UserId == request.CurrentUserId))
            .Where(c => c.Participants.Any(p => p.UserId == request.OtherUserId))
            .Select(c => c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingConversation != Guid.Empty)
        {
            return Result<Guid>.Success(existingConversation);
        }

        // Create new conversation
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            IsGroup = false,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var p1 = new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = request.CurrentUserId,
            JoinedAt = DateTimeOffset.UtcNow
        };

        var p2 = new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = request.OtherUserId,
            JoinedAt = DateTimeOffset.UtcNow
        };

        _context.Conversations.Add(conversation);
        _context.ConversationParticipants.AddRange(p1, p2);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(conversation.Id);
    }
}
