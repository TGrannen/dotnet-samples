using FlurlAPIAccess.TestWebAPI.Services;

namespace FlurlAPIAccess.TestWebAPI.Tests;

public class TestServiceTests
{
    private readonly TestService _sut = new(Options.Create(new APIConfig
    {
        BaseUrl = "https://api.com/"
    }));

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
            Completed = true
        });

        var result = await _sut.GetData(44);

        httpTest.ShouldHaveCalled("https://api.com/todos/44")
            .WithVerb(HttpMethod.Get);
    }
}