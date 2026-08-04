using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Whisper.Domain.Entities;

namespace Whisper.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        try
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            
            await EnsureUserExists(userManager, "user1", "Pass1234");
            await EnsureUserExists(userManager, "user2", "Pass1234");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task EnsureUserExists(UserManager<User> userManager, string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                UserName = username,
                DisplayName = char.ToUpper(username[0]) + username.Substring(1),
                CreatedAt = DateTimeOffset.UtcNow,
                IsOnline = false,
                LastSeen = DateTimeOffset.UtcNow
            };
            await userManager.CreateAsync(user, password);
        }
    }
}
