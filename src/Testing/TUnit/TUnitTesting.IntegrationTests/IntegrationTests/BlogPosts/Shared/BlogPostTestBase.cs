namespace TUnitTesting.IntegrationTests.IntegrationTests.BlogPosts.Shared;

public abstract class BlogPostTestBase : WebApplicationTest<BlogPostWebAppFactory, WebApi.Program>
{
    protected IBlogPostCommentApi CommentAPI { get; private set; } = null!;

    protected IBlogPostApi PostAPI { get; private set; } = null!;

    [Before(Test)]
    public void BeforeTest()
    {
        CommentAPI = Refit.RestService.For<IBlogPostCommentApi>(Factory.CreateClient());
        PostAPI = Refit.RestService.For<IBlogPostApi>(Factory.CreateClient());
    }

    protected async Task ResetBlogPostsAsync()
    {
        await GlobalFactory.Postgres.Respawn();
    }
}
