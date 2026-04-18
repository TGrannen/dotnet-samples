using System.Text.Json;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace TUnitTesting.WireMockTests.Shared;

public static class WireMockExtensions
{
    extension(IRespondWithAProvider responseProvider)
    {
        public void RespondWithJson<T>(T value) => responseProvider.RespondWith(Response.Create()
            .WithStatusCode(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonSerializer.Serialize(value, JsonSerializerOptions.Web)));

        public void RespondWithStatusCode(HttpStatusCode statusCode) => responseProvider.RespondWith(Response.Create().WithStatusCode(statusCode));
        public void RespondWithStatusCode(int statusCode) => responseProvider.RespondWith(Response.Create().WithStatusCode(statusCode));
    }
}