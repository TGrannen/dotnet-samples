namespace Mocking.AutoBogus.StableFaker.TypeGenerators;

internal class StableStringTypeGenerator : IStableTypeGenerator
{
    public Type Type => typeof(string);

    public object Generate(string seed, StableAutoFakerConfig config)
    {
        var faker = StableAutoFakerGenerator.NewFaker(seed, config);
        var lower = seed.ToLowerInvariant();
        if (lower.Contains("firstname"))
            return faker.Name.FirstName();
        if (lower.Contains("lastname"))
            return faker.Name.LastName();
        if (lower.Contains("fullname"))
            return faker.Name.FullName();
        if (lower.Contains("email"))
            return faker.Internet.Email();
        if (lower.Contains("city"))
            return faker.Address.City();
        if (lower.Contains("street"))
            return faker.Address.StreetAddress();
        if (lower.Contains("phone"))
            return faker.Phone.PhoneNumber();
        if (lower.Contains("company"))
            return faker.Company.CompanyName();

        return faker.Lorem.Word();
    }
}