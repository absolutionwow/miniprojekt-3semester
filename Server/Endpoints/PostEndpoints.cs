using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Miniprojekt.Server.Data;
using Miniprojekt.Shared;

namespace Miniprojekt.Server.Endpoints;

public static class PostEndpoints
{
    public static IEndpointRouteBuilder MapPostEndpoints(this IEndpointRouteBuilder app)
    {
        var posts = app.MapGroup("/api/posts");
        posts.MapGet("/", GetPosts);
        posts.MapGet("/{id:int}", GetPost);
        posts.MapPost("/", CreatePost);
        posts.MapPost("/{id:int}/comments", AddComment);
        posts.MapPost("/{id:int}/upvote", (int id, AppDbContext db) => VotePost(id, +1, db));
        posts.MapPost("/{id:int}/downvote", (int id, AppDbContext db) => VotePost(id, -1, db));

        var comments = app.MapGroup("/api/comments");
        comments.MapPost("/{id:int}/upvote", (int id, AppDbContext db) => VoteComment(id, +1, db));
        comments.MapPost("/{id:int}/downvote", (int id, AppDbContext db) => VoteComment(id, -1, db));

        return app;
    }

    // ---------- Reads ----------

    // Front page: newest first.
    private static async Task<List<PostSummaryDto>> GetPosts(AppDbContext db) =>
        await db.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostSummaryDto(
                p.Id, p.Title, p.Url, p.AuthorName, p.CreatedAt, p.Votes, p.Comments.Count))
            .ToListAsync();

    // Thread page: the post and its comments (oldest first, so the thread reads top-down).
    private static async Task<IResult> GetPost(int id, AppDbContext db)
    {
        var post = await db.Posts
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new PostDetailDto(
                p.Id, p.Title, p.Text, p.Url, p.AuthorName, p.CreatedAt, p.Votes,
                p.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDto(c.Id, c.Text, c.AuthorName, c.CreatedAt, c.Votes))
                    .ToList()))
            .FirstOrDefaultAsync();

        return post is null ? Results.NotFound() : Results.Ok(post);
    }

    // ---------- Writes ----------

    private static async Task<IResult> CreatePost(CreatePostRequest request, AppDbContext db)
    {
        if (!TryValidate(request, out var errors))
            return Results.ValidationProblem(errors);

        var hasUrl = !string.IsNullOrWhiteSpace(request.Url);

        var post = new Post
        {
            Title = request.Title.Trim(),
            AuthorName = request.AuthorName.Trim(),
            Text = hasUrl ? null : request.Text!.Trim(),
            Url = hasUrl ? request.Url!.Trim() : null,
            CreatedAt = DateTime.UtcNow,
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();

        var dto = new PostSummaryDto(post.Id, post.Title, post.Url, post.AuthorName, post.CreatedAt, post.Votes, 0);
        return Results.Created($"/api/posts/{post.Id}", dto);
    }

    private static async Task<IResult> AddComment(int id, CreateCommentRequest request, AppDbContext db)
    {
        if (!TryValidate(request, out var errors))
            return Results.ValidationProblem(errors);

        if (!await db.Posts.AnyAsync(p => p.Id == id))
            return Results.NotFound();

        var comment = new Comment
        {
            PostId = id,
            Text = request.Text.Trim(),
            AuthorName = request.AuthorName.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        var dto = new CommentDto(comment.Id, comment.Text, comment.AuthorName, comment.CreatedAt, comment.Votes);
        return Results.Created($"/api/posts/{id}", dto);
    }

    // The increment happens inside the database (UPDATE ... SET Votes = Votes + 1),
    // so two people voting at the same moment never overwrite each other.
    private static async Task<IResult> VotePost(int id, int delta, AppDbContext db)
    {
        var rows = await db.Posts
            .Where(p => p.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Votes, p => p.Votes + delta));

        if (rows == 0) return Results.NotFound();

        var votes = await db.Posts.AsNoTracking().Where(p => p.Id == id).Select(p => p.Votes).SingleAsync();
        return Results.Ok(new VoteResultDto(votes));
    }

    private static async Task<IResult> VoteComment(int id, int delta, AppDbContext db)
    {
        var rows = await db.Comments
            .Where(c => c.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Votes, c => c.Votes + delta));

        if (rows == 0) return Results.NotFound();

        var votes = await db.Comments.AsNoTracking().Where(c => c.Id == id).Select(c => c.Votes).SingleAsync();
        return Results.Ok(new VoteResultDto(votes));
    }

    // ---------- Validation helper ----------

    private static bool TryValidate<T>(T model, out Dictionary<string, string[]> errors) where T : notnull
    {
        var results = new List<ValidationResult>();
        var ok = Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);

        errors = results
            .SelectMany(r =>
            {
                var members = r.MemberNames.Any() ? r.MemberNames : new[] { "" };
                return members.Select(m => (Member: m, Message: r.ErrorMessage ?? "Invalid value."));
            })
            .GroupBy(x => x.Member)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Message).ToArray());

        return ok;
    }
}
