using FlurlAPIAccess.TestWebAPI.Services;

namespace FlurlAPIAccess.TestWebAPI.Tests;

public class TestService2Tests
{
    private readonly TestService2 _sut;

    public TestService2Tests()
    {
        var mock = new Mock<IFlurlRequestProvider>();
        mock.Setup(x => x.GetAuthenticatedRequest()).Returns(() => new FlurlRequest(new Url("https://api.com/")));
        _sut = new TestService2(mock.Object);
    }

    [Fact]
    public async Task Test1()
    {
        // fake & record all http calls in the test subject
        using var httpTest = new HttpTest();
        httpTest.RespondWithJson(new
        {
            UserId = 60,
            Id = 44,
            Title = "TEST",
        });

        var result = await _sut.GetData(44);

        httpTest.ShouldHaveCalled("https://api.com/posts/44")
            .WithVerb(HttpMethod.Get);

        result.Should().BeEquivalentTo(new
        {
            UserId = 60,
            Id = 44,
            Title = "TEST"
        });
    }
}