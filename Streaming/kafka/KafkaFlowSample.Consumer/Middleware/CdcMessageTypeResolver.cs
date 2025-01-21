using KafkaFlow.Middlewares.Serializer.Resolvers;

namespace KafkaFlowSample.Consumer.Middleware;

internal class CdcMessageTypeResolver : IMessageTypeResolver
{
    private readonly Type _type;

    public CdcMessageTypeResolver(string typeName)
    {
        var genericType = Type.GetType(typeName);
        if (genericType == null)
        {
            throw new ArgumentException(
                $"CDC Type was not found for name: \"{typeName}\". Check appsettings to ensure that the correct type name is congifured");
        }

        _type = typeof(DebeziumMessage<>).MakeGenericType(genericType);
    }

    public ValueTask<Type> OnConsumeAsync(IMessageContext context)
    {
        // using var memoryStream = new MemoryStream((context.Message.Value as byte[])!);
        // var model = JsonSerializer.Deserialize<CdcTemp>(memoryStream);
        // typeName.TryGetValue(model!.Source.Table, out string typeString);
        // var type = typeString is null ? null : Type.GetType(typeString);
        return new ValueTask<Type>(_type);
    }

    public ValueTask OnProduceAsync(IMessageContext context)
    {
        throw new NotImplementedException("Not implemented on purpose");
    }
}