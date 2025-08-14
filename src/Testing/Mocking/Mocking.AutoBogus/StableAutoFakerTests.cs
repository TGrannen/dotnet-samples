using Mocking.AutoBogus.StableFaker;

namespace Mocking.AutoBogus;

public class StableAutoFakerTests
{
    [Fact]
    public Task FakePerson()
    {
        var personFaker = new StableAutoFaker<Person>()
            .WithConfiguration(config =>
            {
                // Example
                // config.WithGlobalSeed(640568)
                //     .WithPropertyRule("Email", _ => "fixed@example.com")
                //     .WithTypeRule<decimal>(_ => 999.99m)
                //     .Ignore("CreatedAt");
            });

        var person = personFaker.Generate();

        return Verify(person);
    }
}