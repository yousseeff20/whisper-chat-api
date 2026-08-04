using MediatR;
using Whisper.Domain.Common;

namespace Whisper.Application.Features.Messages.Queries.GetMessages;

public record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string? Text,
    string MessageType,
    bool IsSeen,
    DateTimeOffset SentAt,
    DateTimeOffset? EditedAt,
    DateTimeOffset? DeletedAt,
    bool IsDeletedForEveryone,
    Guid? ReplyToMessageId,
    string? FileUrl,
    string? ImageUrl,
    string? ThumbnailUrl,
    string? FileName,
    long? FileSize,
    string? MimeType
);

public record GetMessagesQuery(Guid ConversationId, Guid UserId, int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedList<MessageDto>>>;

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
