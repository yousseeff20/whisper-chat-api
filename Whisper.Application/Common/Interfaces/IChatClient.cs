using System.Threading.Tasks;

namespace Whisper.Application.Common.Interfaces;

public interface IChatClient
{
    Task ReceiveMessage(object message);
    Task Typing(string conversationId, string userId);
    Task StopTyping(string conversationId, string userId);
    Task Seen(string conversationId, string messageId, string userId);
    Task MessageEdited(object message);
    Task MessageDeleted(string messageId);
    Task UserConnected(string userId);
    Task UserDisconnected(string userId);
}
