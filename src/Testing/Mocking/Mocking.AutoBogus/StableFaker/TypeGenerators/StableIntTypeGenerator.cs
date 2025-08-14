namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableIntTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(int);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Random.Int(1, 1000);
    }
}