# miniprojekt-3semester – Web API + Blazor WebAssembly + EF Core

Hej Peter. Her er min aflevering. Jeg har desværre haft en depression, så har kun kunne følge med hjemmefra. Jeg håber på snart at være tilbage på studiet. </br>Mvh Andreas Dahl Pedersen </br></br>
A simple discussion forum: create threads (text or link), comment on them and up/downvote both.

## Stack
| Layer | Tech |
|-------|------|
| API | ASP.NET Core 10 **Minimal API** (`Server`) |
| UI | **Blazor WebAssembly** (`Client`), served by the server |
| Data | **Entity Framework Core** + SQLite (`miniprojekt.db`) |
| Shared | DTOs + validated request models (`Shared`) |

## First-time setup (migration)
The `Migrations` folder must exist and be committed – generate it once:
```bash
cd Server
dotnet ef migrations add InitialCreate
```

## Run it
Requires the .NET 10 SDK.</br>
The dotnet commands should be used inside the miniprojekt-3semester folder that

```bash
dotnet tool restore            # installs dotnet-ef (once)
dotnet build                   # from the repository root
cd Server
dotnet ef database update
dotnet run
```
Open http://localhost:5080. Test data is created automatically when the database is empty.



## API
| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/posts` | Threads, newest first |
| GET | `/api/posts/{id}` | One thread + comments |
| POST | `/api/posts` | Create thread |
| POST | `/api/posts/{id}/comments` | Add comment |
| POST | `/api/posts/{id}/upvote` · `/downvote` | Vote on thread |
| POST | `/api/comments/{id}/upvote` · `/downvote` | Vote on comment |

## Group members
Andreas Dahl Pedersen
