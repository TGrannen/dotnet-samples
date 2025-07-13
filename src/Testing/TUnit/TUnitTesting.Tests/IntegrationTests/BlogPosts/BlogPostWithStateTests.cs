﻿namespace TUnitTesting.Tests.IntegrationTests.BlogPosts;

/// <summary>
/// Same Tests as <see cref="BlogPostTests"/> but instead of creating new posts for every test, a post is created by
/// one test and then reference to that post is passed through the ObjectBag to dependent tests
/// </summary>
[Category("Integration")]
public class BlogPostWithStateTests
{
    [ClassDataSource<BlogPostWebAppFactory>(Shared = SharedType.PerTestSession)]
    public required BlogPostWebAppFactory Factory { get; init; }

    [Test]
    public async Task CreatePost_ReturnsCreatedStatusCodeAndPost()
    {
        var newPost = new { Title = "New Pure TUnit Post", Content = "This is a new test post with TUnit." };

        var response = await Factory.PostAPI.CreatePost(newPost);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Created);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content.Title).IsEqualTo(newPost.Title);
        await Assert.That(response.Content.Content).IsEqualTo(newPost.Content);

        TestContext.Current!.ObjectBag.Add("PostId", response.Content.Id);
    }

    [Test]
    [DependsOn(nameof(CreatePost_ReturnsCreatedStatusCodeAndPost))]
    public async Task GetPosts_ReturnsSuccessStatusCodeAndPosts()
    {
        var response = await Factory.PostAPI.GetPosts();

        var postId = GetPostId();
        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response.Content).IsNotNull();
        await Assert.That(response.Content).Contains(x => x.Id == postId);
        await Assert.That(response.Content!.First(x => x.Id == postId).Title).IsEqualTo("New Pure TUnit Post");
    }

    [Test]
    [DependsOn(nameof(GetPosts_ReturnsSuccessStatusCodeAndPosts))]
    public async Task UpdatePost_ReturnsNoContent()
    {
        var updatedPostDto = new { Title = "Pure TUnit Updated Title", Content = "Pure TUnit Updated content." };
        var postId = GetPostId();
        var response = await Factory.PostAPI.UpdatePost(postId, updatedPostDto);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task GetPostById_ReturnsNotFound_ForNonExistentPost()
    {
        var response = await Factory.PostAPI.GetPostById(99999);

        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    [DependsOn(nameof(UpdatePost_ReturnsNoContent))]
    public async Task DeletePost_ReturnsNoContent()
    {
        var postId = GetPostId();
        var response = await Factory.PostAPI.DeletePost(postId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(true);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
    }

    [Test]
    [DependsOn(nameof(DeletePost_ReturnsNoContent))]
    public async Task DeletePost_ReturnsNotFound_AfterDeleted()
    {
        var postId = GetPostId();
        var response = await Factory.PostAPI.GetPostById(postId);

        using var _ = Assert.Multiple();
        await Assert.That(response.IsSuccessStatusCode).IsEqualTo(false);
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    private static int GetPostId()
    {
        var addToBagTestContext = TestContext.Current!.GetTests(nameof(CreatePost_ReturnsCreatedStatusCodeAndPost)).First();
        var postId = addToBagTestContext.ObjectBag["PostId"] as int? ?? -1;
        return postId;
    }
}