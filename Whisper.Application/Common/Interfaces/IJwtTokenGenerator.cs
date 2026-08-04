using System.Threading.Tasks;
using Whisper.Domain.Entities;

namespace Whisper.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
