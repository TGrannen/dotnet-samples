using Refit;

namespace TUnitTesting.Tests.IntegrationTests.BlogPosts.Shared;

public interface IBlogPostApi
{
    [Post("/api/Posts")]
    Task<ApiResponse<PostResponseDto>> CreatePost([Body] object post);

    [Get("/api/Posts")]
    Task<ApiResponse<PostResponseDto[]>> GetPosts();

    [Get("/api/Posts/{id}")]
    Task<ApiResponse<PostResponseDto>> GetPostById(int id);

    [Put("/api/Posts/{id}")]
    Task<ApiResponse<object>> UpdatePost(int id, [Body] object post);

    [Delete("/api/Posts/{id}")]
    Task<ApiResponse<object>> DeletePost(int id);
}

public class PostResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Optionally include comments, but they will be CommentResponseDto
    public ICollection<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
}

public class CommentResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int PostId { get; set; } // We might still want to know the parent PostId
}