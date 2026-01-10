namespace TUnitTesting.Tests.IntegrationTests.BlogPosts;

[Category("Integration")]
public class BlogPostTests : TestsBase
{
    [Test]
    public async Task CreatePost_ReturnsCreatedStatusCodeAndPost()
    {
        var newPost = new { Title = "New Pure TUnit Post", Content = "This is a new test post with TUnit." };
        var response = await PostAPI.CreatePost(newPost);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content.Title).IsEqualTo(newPost.Title);
        await Assert.That(response.Content.Content).IsEqualTo(newPost.Content);
    }

    [Test]
    [NotInParallel]
    public async Task GetPosts_ReturnsSuccessStatusCodeAndPosts()
    {
        await ResetBlogPostsAsync();
        var postId = await CreatePost("New Pure TUnit Post");

        var response = await PostAPI.GetPosts();

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content).Contains(x => x.Id == postId);
        await Assert.That(response.Content!.First(x => x.Id == postId).Title).IsEqualTo("New Pure TUnit Post");
    }

    [Test]
    public async Task UpdatePost_ReturnsNoContent()
    {
        var updatedPostDto = new { Title = "Pure TUnit Updated Title", Content = "Pure TUnit Updated content." };
        var postId = await CreatePost();

        var response = await PostAPI.UpdatePost(postId, updatedPostDto);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task GetPostById_ReturnsNotFound_ForNonExistentPost()
    {
        var response = await PostAPI.GetPostById(99999);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeletePost_ReturnsNoContent()
    {
        var postId = await CreatePost();

        var response = await PostAPI.DeletePost(postId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeletePost_ReturnsNotFound_AfterDeleted()
    {
        var postId = await CreatePost();
        await PostAPI.DeletePost(postId);

        var response = await PostAPI.GetPostById(postId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(false);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    private async Task<int> CreatePost(string title = "Dummy Post")
    {
        var response = await PostAPI.CreatePost(new { Title = title, Content = "This is a new test post with TUnit." });
        return response.Content!.Id;
    }
}