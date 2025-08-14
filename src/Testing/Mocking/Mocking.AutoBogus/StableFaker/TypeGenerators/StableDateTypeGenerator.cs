namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableDateTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(DateTime);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Date.Past(10);
    }
}