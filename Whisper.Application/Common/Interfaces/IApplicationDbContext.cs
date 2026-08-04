using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Whisper.Domain.Entities;

namespace Whisper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Conversation> Conversations { get; }
    DbSet<Message> Messages { get; }
    DbSet<ConversationParticipant> ConversationParticipants { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
