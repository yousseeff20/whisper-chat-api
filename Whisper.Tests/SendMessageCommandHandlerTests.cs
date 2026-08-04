using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Whisper.Application.Common.Interfaces;
using Whisper.Application.Features.Messages.Commands.SendMessage;
using Whisper.Domain.Entities;
using Whisper.Domain.Enums;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Whisper.Tests.Application.Features.Messages.Commands;

public class SendMessageCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenMessageIsSentSuccessfully()
    {
        // Arrange
        var contextMock = new Mock<IApplicationDbContext>();
        var notifierMock = new Mock<IRealtimeNotifier>();

        var options = new DbContextOptionsBuilder<Whisper.Infrastructure.Persistence.ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new Whisper.Infrastructure.Persistence.ApplicationDbContext(options);

        var handler = new SendMessageCommandHandler(context, notifierMock.Object);

        var senderId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();

        context.Users.Add(new User { Id = senderId, UserName = "testuser" });
        var conv = new Conversation { Id = conversationId };
        context.Conversations.Add(conv);
        context.ConversationParticipants.Add(new ConversationParticipant { ConversationId = conversationId, UserId = senderId });
        await context.SaveChangesAsync(CancellationToken.None);

        var command = new SendMessageCommand("Hello World", conversationId.ToString(), senderId.ToString());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Hello World", result.Value.Content);
        
        notifierMock.Verify(n => n.NotifyMessageReceivedAsync(conversationId.ToString(), It.IsAny<SendMessageResponse>()), Times.Once);
    }
}
