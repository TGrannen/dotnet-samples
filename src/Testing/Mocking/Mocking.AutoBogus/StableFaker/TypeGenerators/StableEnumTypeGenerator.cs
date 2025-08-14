namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableEnumTypeGenerator(Type enumType) : IStableTypeGenerator
{
    public Type Type => enumType;

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        var values = Enum.GetValues(enumType);
        return values.GetValue(faker.Random.Int(0, values.Length - 1))!;
    }
}