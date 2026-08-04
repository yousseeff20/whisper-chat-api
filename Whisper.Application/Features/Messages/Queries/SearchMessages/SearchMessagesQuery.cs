using MediatR;
using Whisper.Domain.Common;
using Whisper.Application.Features.Messages.Queries.GetMessages;

namespace Whisper.Application.Features.Messages.Queries.SearchMessages;

public record SearchMessagesQuery(string Query, Guid UserId, int Page = 1, int PageSize = 50) : IRequest<Result<PaginatedList<MessageDto>>>;
