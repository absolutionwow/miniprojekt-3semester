using System.Net;
using System.Net.Http.Json;
using Miniprojekt.Shared;

namespace Miniprojekt.Client.Services;

/// <summary>Thin wrapper around HttpClient so pages never build URLs themselves.</summary>
public class ApiClient(HttpClient http)
{
    public async Task<List<PostSummaryDto>> GetPostsAsync() =>
        await http.GetFromJsonAsync<List<PostSummaryDto>>("api/posts") ?? new();

    /// <summary>Returns null when the thread does not exist.</summary>
    public async Task<PostDetailDto?> GetPostAsync(int id)
    {
        var response = await http.GetAsync($"api/posts/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PostDetailDto>();
    }

    public async Task<PostSummaryDto> CreatePostAsync(CreatePostRequest request)
    {
        var response = await http.PostAsJsonAsync("api/posts", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PostSummaryDto>())!;
    }

    public async Task<CommentDto> AddCommentAsync(int postId, CreateCommentRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/posts/{postId}/comments", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CommentDto>())!;
    }

    /// <summary>Returns the new vote total.</summary>
    public Task<int> VotePostAsync(int postId, bool up) =>
        VoteAsync($"api/posts/{postId}/{(up ? "upvote" : "downvote")}");

    public Task<int> VoteCommentAsync(int commentId, bool up) =>
        VoteAsync($"api/comments/{commentId}/{(up ? "upvote" : "downvote")}");

    private async Task<int> VoteAsync(string url)
    {
        var response = await http.PostAsync(url, content: null);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VoteResultDto>();
        return result!.Votes;
    }
}
