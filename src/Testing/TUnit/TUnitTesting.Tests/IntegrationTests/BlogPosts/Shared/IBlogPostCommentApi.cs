using Refit;

namespace TUnitTesting.Tests.IntegrationTests.BlogPosts.Shared;

public interface IBlogPostCommentApi
{
    [Post("/api/Posts")]
    Task<ApiResponse<PostResponseDto>> CreatePost([Body] object post);

    [Post("/api/posts/{postId}/comments")]
    Task<ApiResponse<CommentResponseDto>> CreateComment(int postId, [Body] object comment);

    [Get("/api/posts/{postId}/comments")]
    Task<ApiResponse<CommentResponseDto[]>> GetComments(int postId);

    [Delete("/api/posts/{postId}/comments/{commentId}")]
    Task<ApiResponse<object>> DeleteComment(int postId, int commentId);
}