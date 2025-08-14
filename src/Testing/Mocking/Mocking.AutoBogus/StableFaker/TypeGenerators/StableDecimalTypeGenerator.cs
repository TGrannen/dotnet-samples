namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableDecimalTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(decimal);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Random.Decimal(1, 1000);
    }
}