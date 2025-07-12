namespace TUnitTesting.Tests.IntegrationTests;

[Property("Category", "Integration")]
public class BlogPostCommentTests
{
    private IBlogPostCommentApi API => Refit.RestService.For<IBlogPostCommentApi>(Factory.CreateClient());

    [ClassDataSource<WebAppFactory>(Shared = SharedType.PerTestSession)]
    public required WebAppFactory Factory { get; init; }

    [Test]
    public async Task CreatePost_ReturnsCreatedStatusCodeAndPost()
    {
        var newPost = new { Title = "New Pure TUnit Post", Content = "This is a new test post with TUnit." };

        var response = await API.CreatePost(newPost);

        using var _ = Assert.Multiple();

        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();

        TestContext.Current!.ObjectBag.Add("PostId", response.Content.Id);
    }

    [Test]
    [DependsOn(nameof(CreatePost_ReturnsCreatedStatusCodeAndPost))]
    public async Task CreateComment_ReturnsCreatedStatusCodeAndComment()
    {
        var newCommentDto = new { Text = "This is a pure TUnit test comment." };
        var postId = GetPostId();

        var response = await API.CreateComment(postId, newCommentDto);

        using var _ = Assert.Multiple();

        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content.Text).IsEqualTo(newCommentDto.Text);
        await Assert.That(response.Content.PostId).IsEqualTo(postId);

        TestContext.Current!.ObjectBag.Add("CommentId", response.Content.Id);
    }

    [Test]
    [DependsOn(nameof(CreateComment_ReturnsCreatedStatusCodeAndComment))]
    public async Task GetCommentsForPost_ReturnsComments()
    {
        var postId = GetPostId();
        var response = await API.GetComments(postId);

        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);

        using var _ = Assert.Multiple();
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content).HasCount(1);
        await Assert.That(response.Content).Contains(c => c.Text == "This is a pure TUnit test comment.");
    }

    [Test]
    [DependsOn(nameof(GetCommentsForPost_ReturnsComments))]
    public async Task DeleteComment_ReturnsNoContent()
    {
        var postId = GetPostId();
        var commentId = GetCommentId();
        var response = await API.DeleteComment(postId, commentId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    private static int GetPostId()
    {
        var addToBagTestContext = TestContext.Current!.GetTests(nameof(CreatePost_ReturnsCreatedStatusCodeAndPost)).First();
        var postId = addToBagTestContext.ObjectBag["PostId"] as int? ?? -1;
        return postId;
    }

    private static int GetCommentId()
    {
        var addToBagTestContext = TestContext.Current!.GetTests(nameof(CreateComment_ReturnsCreatedStatusCodeAndComment)).First();
        var postId = addToBagTestContext.ObjectBag["CommentId"] as int? ?? -1;
        return postId;
    }
}