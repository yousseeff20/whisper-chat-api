using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Queries.GetMessages;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Result<PaginatedList<MessageDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<MessageDto>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var isParticipant = await _context.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == request.ConversationId && cp.UserId == request.UserId, cancellationToken);

        if (!isParticipant)
            return Result<PaginatedList<MessageDto>>.Failure<PaginatedList<MessageDto>>(new Error("Conversation.Unauthorized", "Not authorized to view these messages."));

        var query = _context.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == request.ConversationId)
            .OrderByDescending(m => m.SentAt);

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageDto(
                m.Id,
                m.ConversationId,
                m.SenderId,
                m.Text,
                m.MessageType.ToString(),
                m.IsSeen,
                m.SentAt,
                m.EditedAt,
                m.DeletedAt,
                m.IsDeletedForEveryone,
                m.ReplyToMessageId,
                m.FileUrl,
                m.ImageUrl,
                m.ThumbnailUrl,
                m.FileName,
                m.FileSize,
                m.MimeType
            ))
            .ToListAsync(cancellationToken);

        // Reverse to chronological order for client (optional, but typical for chat history loaded backwards)
        // items.Reverse(); // Usually frontend handles this, but let's keep it descending for generic pagination.

        var paginatedList = new PaginatedList<MessageDto>(items, count, request.Page, request.PageSize);
        return Result<PaginatedList<MessageDto>>.Success(paginatedList);
    }
}
