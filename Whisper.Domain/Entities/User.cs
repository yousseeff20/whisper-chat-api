using System;
using Microsoft.AspNetCore.Identity;

namespace Whisper.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public bool IsOnline { get; set; }
    public DateTimeOffset LastSeen { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
