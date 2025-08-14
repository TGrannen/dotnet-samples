namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableTimeSpanTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(TimeSpan);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        return faker.Date.Timespan();
    }
}