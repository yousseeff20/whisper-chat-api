using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;
using Whisper.Application.Features.Messages.Queries.GetMessages;

namespace Whisper.Application.Features.Messages.Queries.SearchMessages;

public class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQuery, Result<PaginatedList<MessageDto>>>
{
    private readonly IApplicationDbContext _context;

    public SearchMessagesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<MessageDto>>> Handle(SearchMessagesQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Result<PaginatedList<MessageDto>>.Success(new PaginatedList<MessageDto>(new List<MessageDto>(), 0, request.Page, request.PageSize));

        var searchTerm = $"%{request.Query}%";

        // Find conversations the user is part of
        var userConversationIds = await _context.ConversationParticipants
            .Where(cp => cp.UserId == request.UserId)
            .Select(cp => cp.ConversationId)
            .ToListAsync(cancellationToken);

        var query = _context.Messages
            .AsNoTracking()
            .Where(m => userConversationIds.Contains(m.ConversationId) && !m.IsDeletedForEveryone && m.Text != null && m.Text.Contains(request.Query))
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

        var paginatedList = new PaginatedList<MessageDto>(items, count, request.Page, request.PageSize);
        return Result<PaginatedList<MessageDto>>.Success(paginatedList);
    }
}
