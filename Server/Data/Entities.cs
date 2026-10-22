namespace Miniprojekt.Server.Data;

/// <summary>
/// A thread. Named "Post" to avoid clashing with System.Threading.Thread.
/// Exactly one of Text / Url is filled in.
/// </summary>
public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Text { get; set; }
    public string? Url { get; set; }
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int Votes { get; set; }

    public List<Comment> Comments { get; set; } = new();
}

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int Votes { get; set; }

    public int PostId { get; set; }
    public Post? Post { get; set; }
}
