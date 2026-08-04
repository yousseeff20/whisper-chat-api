using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Whisper.Application.Common.Interfaces;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Conversations.Queries.GetConversations;

public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, Result<PaginatedList<ConversationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetConversationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ConversationDto>>> Handle(GetConversationsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Conversations
            .AsNoTracking()
            .Where(c => c.Participants.Any(p => p.UserId == request.UserId))
            .OrderByDescending(c => c.UpdatedAt);

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ConversationDto(
                c.Id,
                c.Title,
                c.IsGroup,
                c.CreatedAt,
                c.UpdatedAt,
                c.Messages.OrderByDescending(m => m.SentAt).Select(m => new MessageDto(
                    m.Id,
                    m.SenderId,
                    m.Text,
                    m.MessageType.ToString(),
                    m.IsSeen,
                    m.SentAt,
                    m.IsDeletedForEveryone
                )).FirstOrDefault(),
                c.Participants.Select(p => new ParticipantDto(
                    p.UserId,
                    p.User.UserName!,
                    p.User.Avatar,
                    p.JoinedAt
                )).ToList()
            ))
            .ToListAsync(cancellationToken);

        var paginatedList = new PaginatedList<ConversationDto>(items, count, request.Page, request.PageSize);
        return Result<PaginatedList<ConversationDto>>.Success(paginatedList);
    }
}
