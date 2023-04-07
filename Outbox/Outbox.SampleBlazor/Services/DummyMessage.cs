using System.Text.Json;

namespace Outbox.SampleBlazor.Services;

internal class DummyMessage : IMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public object? Body { get; init; }
    public string Payload => JsonSerializer.Serialize(Body);
}