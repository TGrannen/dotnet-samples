namespace TUnitTesting.Tests.IntegrationTests.BlogPosts.Shared;

public abstract class BlogPostTestBase : WebApplicationTest<BlogPostWebAppFactory, Program>
{
    [ClassDataSource<PostgresContainer>(Shared = SharedType.PerTestSession)]
    private PostgresContainer Postgres { get; init; } = null!;

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
        await Postgres.Respawn();
    }
}