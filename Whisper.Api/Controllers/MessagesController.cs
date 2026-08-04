using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Whisper.Application.Features.Messages.Commands.SendMessage;
using Whisper.Application.Features.Messages.Commands.EditMessage;
using Whisper.Application.Features.Messages.Commands.DeleteMessage;
using Whisper.Application.Features.Messages.Commands.MarkMessageSeen;
using Whisper.Application.Features.Messages.Queries.GetMessages;
using Whisper.Application.Features.Messages.Queries.SearchMessages;
using Whisper.Domain.Enums;

namespace Whisper.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("conversation/{conversationId}")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new GetMessagesQuery(conversationId, userId, page, pageSize));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new SearchMessagesQuery(query, userId, page, pageSize));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var command = new SendMessageCommand(
            dto.Content, 
            dto.ConversationId, 
            userId.ToString(),
            dto.ReplyToMessageId,
            dto.MessageType ?? MessageType.Text,
            dto.FileUrl, dto.ImageUrl, dto.ThumbnailUrl, dto.FileName, dto.FileSize, dto.MimeType);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditMessage(Guid id, [FromBody] EditMessageDto dto)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new EditMessageCommand(id, dto.NewText, userId));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new DeleteMessageCommand(id, userId));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{id}/seen")]
    public async Task<IActionResult> MarkMessageSeen(Guid id)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _mediator.Send(new MarkMessageSeenCommand(id, userId));
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    private Guid GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
    }
}

public record SendMessageDto(
    string Content, 
    string ConversationId, 
    Guid? ReplyToMessageId, 
    MessageType? MessageType,
    string? FileUrl, 
    string? ImageUrl, 
    string? ThumbnailUrl, 
    string? FileName, 
    long? FileSize, 
    string? MimeType);

public record EditMessageDto(string NewText);
