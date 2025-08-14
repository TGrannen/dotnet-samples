namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableDoubleTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(double);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Random.Double(1, 1000);
    }
}