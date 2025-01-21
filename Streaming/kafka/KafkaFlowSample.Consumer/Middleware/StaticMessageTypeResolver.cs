using KafkaFlow.Middlewares.Serializer.Resolvers;

namespace KafkaFlowSample.Consumer.Middleware;

internal class StaticMessageTypeResolver : IMessageTypeResolver
{
    private readonly Type? _type;

    // TODO TEST THIS
    public StaticMessageTypeResolver(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
        {
            return;
        }

        _type = Type.GetType(typeName);
        if (_type == null)
        {
            throw new ArgumentException(
                $"Static Type was not found for name: \"{typeName}\". Check appsettings to ensure that the correct type name is congifured");
        }
    }

    public ValueTask<Type> OnConsumeAsync(IMessageContext context)
    {
        return new ValueTask<Type>(_type);
    }

    public ValueTask OnProduceAsync(IMessageContext context)
    {
        throw new NotImplementedException("Not implemented on purpose");
    }
}