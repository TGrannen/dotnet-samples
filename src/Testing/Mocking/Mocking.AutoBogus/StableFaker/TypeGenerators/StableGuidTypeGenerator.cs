using System.Security.Cryptography;
using System.Text;

namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableGuidTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(Guid);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var combined = config.GlobalSeed != null
            ? $"{config.GlobalSeed.Value}:{seed}"
            : seed;

        var bytes = Encoding.UTF8.GetBytes(combined.ToLowerInvariant());
        var hash = MD5.HashData(bytes);
        return new Guid(hash);
    }
}