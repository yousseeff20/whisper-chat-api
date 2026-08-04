using System;
using System.Threading.Tasks;
using Whisper.Application.Features.Messages.Commands.SendMessage;

namespace Whisper.Application.Common.Interfaces;

public interface IRealtimeNotifier
{
    Task NotifyMessageReceivedAsync(string conversationId, SendMessageResponse message);
    Task NotifyMessageEditedAsync(Guid conversationId, Guid messageId, string newText);
    Task NotifyMessageDeletedAsync(Guid conversationId, Guid messageId);
    Task NotifyMessageSeenAsync(Guid conversationId, Guid messageId, Guid userId);
}
