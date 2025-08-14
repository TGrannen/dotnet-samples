namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal interface IStableTypeGenerator
{
    Type Type { get; }
    object Generate(string seed, StableAutoFakerConfig config);
}