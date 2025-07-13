namespace TUnitTesting.Tests.IntegrationTests.BlogPosts;

[Category("Integration")]
public class BlogPostCommentTests
{
    [ClassDataSource<BlogPostWebAppFactory>(Shared = SharedType.PerTestSession)]
    public required BlogPostWebAppFactory Factory { get; init; }

    [Test]
    public async Task CreatePost_ReturnsCreatedStatusCodeAndPost()
    {
        var newPost = new { Title = "New Pure TUnit Post", Content = "This is a new test post with TUnit." };
        var response = await Factory.CommentAPI.CreatePost(newPost);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();
    }

    [Test]
    public async Task CreateComment_ReturnsCreatedStatusCodeAndComment()
    {
        var newCommentDto = new { Text = "This is a pure TUnit test comment." };
        var postId = await CreatePost();

        var response = await Factory.CommentAPI.CreateComment(postId, newCommentDto);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content.Text).IsEqualTo(newCommentDto.Text);
        await Assert.That(response.Content.PostId).IsEqualTo(postId);
    }

    [Test]
    public async Task GetCommentsForPost_ReturnsComments()
    {
        var postId = await CreatePost();
        await CreateComment(postId);

        var response = await Factory.CommentAPI.GetComments(postId);

        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);

        using var _ = Assert.Multiple();
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content).HasCount(1);
        await Assert.That(response.Content).Contains(c => c.Text == "This is a pure TUnit test comment.");
    }

    [Test]
    public async Task DeleteComment_ReturnsNoContent()
    {
        var postId = await CreatePost();
        var commentId = await CreateComment(postId);
        var response = await Factory.CommentAPI.DeleteComment(postId, commentId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    private async Task<int> CreatePost()
    {
        var response = await Factory.CommentAPI.CreatePost(new { Title = "Dummy Post", Content = "This is a new test post with TUnit." });
        return response.Content!.Id;
    }

    private async Task<int> CreateComment(int postId, string? text = null)
    {
        var response = await Factory.CommentAPI.CreateComment(postId, new { Text = text ?? "This is a pure TUnit test comment." });
        return response.Content!.Id;
    }
}