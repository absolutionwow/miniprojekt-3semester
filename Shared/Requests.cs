using System.ComponentModel.DataAnnotations;

namespace Miniprojekt.Shared;

// ----- Requests (client -> server) -----
// These are shared so the same validation rules run in the Blazor form
// (instant feedback) AND on the server (the real security boundary).

public class CreatePostRequest : IValidatableObject
{
    [Required(ErrorMessage = "Author name is required.")]
    [StringLength(50, ErrorMessage = "Author name can be at most 50 characters.")]
    public string AuthorName { get; set; } = "";

    [Required(ErrorMessage = "A title is required.")]
    [StringLength(200, ErrorMessage = "Title can be at most 200 characters.")]
    public string Title { get; set; } = "";

    [StringLength(10000, ErrorMessage = "Text can be at most 10,000 characters.")]
    public string? Text { get; set; }

    [StringLength(2000, ErrorMessage = "URL can be at most 2,000 characters.")]
    public string? Url { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasText = !string.IsNullOrWhiteSpace(Text);
        var hasUrl = !string.IsNullOrWhiteSpace(Url);

        if (hasText == hasUrl)
        {
            yield return new ValidationResult(
                "A thread needs either some text or a URL (not both, not neither).",
                new[] { nameof(Text), nameof(Url) });
        }
        else if (hasUrl)
        {
            var ok = Uri.TryCreate(Url!.Trim(), UriKind.Absolute, out var uri)
                     && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            if (!ok)
            {
                yield return new ValidationResult(
                    "The URL must be a valid http:// or https:// address.",
                    new[] { nameof(Url) });
            }
        }
    }
}

public class CreateCommentRequest
{
    [Required(ErrorMessage = "Author name is required.")]
    [StringLength(50, ErrorMessage = "Author name can be at most 50 characters.")]
    public string AuthorName { get; set; } = "";

    [Required(ErrorMessage = "Write something first.")]
    [StringLength(5000, ErrorMessage = "A comment can be at most 5,000 characters.")]
    public string Text { get; set; } = "";
}
