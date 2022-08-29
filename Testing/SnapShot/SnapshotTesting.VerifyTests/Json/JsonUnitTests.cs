using System.Text;
using Newtonsoft.Json.Linq;

namespace SnapshotTesting.VerifyTests.Json;

[UsesVerify]
public class JsonUnitTests
{
    [Fact]
    public Task VerifyJsonString()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        return VerifyJson(json);
    }

    [Fact]
    public Task VerifyJsonStream()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        return VerifyJson(stream);
    }

    [Fact]
    public Task StreamMember()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes("value"));
        return Verify(new { stream });
    }

    [Fact]
    public Task VerifyJsonJToken()
    {
        var json = "{'key': {'msg': 'No action taken'}}";
        var target = JToken.Parse(json);
        return Verify(target);
    }
}