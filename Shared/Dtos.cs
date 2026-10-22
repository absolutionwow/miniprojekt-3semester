namespace Miniprojekt.Shared;

// ----- Responses (server -> client) -----

/// <summary>One row on the front page.</summary>
public record PostSummaryDto(
    int Id,
    string Title,
    string? Url,
    string AuthorName,
    DateTime CreatedAt,
    int Votes,
    int CommentCount);

public record CommentDto(
    int Id,
    string Text,
    string AuthorName,
    DateTime CreatedAt,
    int Votes);

/// <summary>A single thread including all of its comments (oldest first).</summary>
public record PostDetailDto(
    int Id,
    string Title,
    string? Text,
    string? Url,
    string AuthorName,
    DateTime CreatedAt,
    int Votes,
    List<CommentDto> Comments);

public record VoteResultDto(int Votes);
