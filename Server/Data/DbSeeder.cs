using Microsoft.EntityFrameworkCore;

namespace Miniprojekt.Server.Data;

public static class DbSeeder
{
    /// <summary>Adds test data, but only when the database is empty.</summary>
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Posts.AnyAsync())
            return;

        var now = DateTime.UtcNow;

        var posts = new List<Post>
        {
            new()
            {
                Title = "Welcome to Miniprojekt!",
                Text = "This is a small discussion forum built with ASP.NET Core Minimal API, Blazor WebAssembly and Entity Framework.\n\nTry voting, commenting and creating your own thread.",
                AuthorName = "Admin",
                CreatedAt = now.AddHours(-30),
                Votes = 12,
                Comments =
                {
                    new() { Text = "First! Looks great.", AuthorName = "Alice", CreatedAt = now.AddHours(-29), Votes = 3 },
                    new() { Text = "Is there a login? I can't find it.", AuthorName = "Bob", CreatedAt = now.AddHours(-28), Votes = 1 },
                    new() { Text = "No login - everything is anonymous.", AuthorName = "Admin", CreatedAt = now.AddHours(-27), Votes = 4 },
                }
            },
            new()
            {
                Title = "Official .NET documentation",
                Url = "https://learn.microsoft.com/dotnet/",
                AuthorName = "Charlie",
                CreatedAt = now.AddHours(-8),
                Votes = 7,
                Comments =
                {
                    new() { Text = "Bookmarked.", AuthorName = "Dana", CreatedAt = now.AddHours(-7), Votes = 0 },
                }
            },
            new()
            {
                Title = "What is your favourite C# feature?",
                Text = "Records, pattern matching, LINQ, async/await... I can't decide. What do you think?",
                AuthorName = "Dana",
                CreatedAt = now.AddHours(-3),
                Votes = -1,
                Comments =
                {
                    new() { Text = "LINQ, no contest.", AuthorName = "Eve", CreatedAt = now.AddHours(-2), Votes = 2 },
                    new() { Text = "Records. So much less boilerplate.", AuthorName = "Frank", CreatedAt = now.AddHours(-1), Votes = 1 },
                }
            },
            new()
            {
                Title = "Blazor WebAssembly explained",
                Url = "https://learn.microsoft.com/aspnet/core/blazor/",
                AuthorName = "Eve",
                CreatedAt = now.AddMinutes(-20),
                Votes = 0,
            },
        };

        db.Posts.AddRange(posts);
        await db.SaveChangesAsync();
    }
}
