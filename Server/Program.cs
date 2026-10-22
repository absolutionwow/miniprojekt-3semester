using Microsoft.EntityFrameworkCore;
using Miniprojekt.Server.Data;
using Miniprojekt.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=miniprojekt.db"));

var app = builder.Build();

// Make sure the database is up to date and contains test data.
// (This runs on startup, so `dotnet run` works even if `dotnet ef database update`
//  has not been run yet. The EF tools stop after Build(), so this is not executed by them.)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.Database.GetMigrations().Any())
    {
        throw new InvalidOperationException(
            "No EF Core migrations found. Run: dotnet ef migrations add InitialCreate " +
            "(from the Miniprojekt.Server folder) and commit the Migrations folder.");
    }

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

// Serve the Blazor WebAssembly client from this same server...
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// ...and expose the Minimal API under /api.
app.MapPostEndpoints();

// Any other URL (e.g. /thread/3 on a browser refresh) gets the Blazor app.
app.MapFallbackToFile("index.html");

app.Run();
