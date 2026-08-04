using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Whisper.Application.Features.Messages.Commands.SendMessage;
using Whisper.Api.Hubs;
using Whisper.Application.Common.Interfaces;

namespace Whisper.Api.Services;

public class RealtimeNotifier : IRealtimeNotifier
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public RealtimeNotifier(IHubContext<ChatHub, IChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyMessageReceivedAsync(string conversationId, SendMessageResponse message)
    {
        await _hubContext.Clients.Group(conversationId).ReceiveMessage(message);
    }

    public async Task NotifyMessageEditedAsync(Guid conversationId, Guid messageId, string newText)
    {
        await _hubContext.Clients.Group(conversationId.ToString()).MessageEdited(new { MessageId = messageId.ToString(), NewText = newText });
    }

    public async Task NotifyMessageDeletedAsync(Guid conversationId, Guid messageId)
    {
        await _hubContext.Clients.Group(conversationId.ToString()).MessageDeleted(messageId.ToString());
    }

    public async Task NotifyMessageSeenAsync(Guid conversationId, Guid messageId, Guid userId)
    {
        await _hubContext.Clients.Group(conversationId.ToString()).Seen(conversationId.ToString(), messageId.ToString(), userId.ToString());
    }
}
