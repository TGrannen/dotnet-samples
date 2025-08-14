namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableBoolTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(bool);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Random.Bool();
    }
}