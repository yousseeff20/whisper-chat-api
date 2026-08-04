using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Conversations.Queries.GetConversations;

public record ConversationDto(
    Guid Id,
    string? Title,
    bool IsGroup,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    MessageDto? LastMessage,
    List<ParticipantDto> Participants
);

public record ParticipantDto(
    Guid UserId,
    string Username,
    string? AvatarUrl,
    DateTimeOffset JoinedAt
);

public record MessageDto(
    Guid Id,
    Guid SenderId,
    string? Text,
    string MessageType,
    bool IsSeen,
    DateTimeOffset SentAt,
    bool IsDeletedForEveryone
);

public record GetConversationsQuery(Guid UserId, int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedList<ConversationDto>>>;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }
}
