namespace Mocking.AutoBogus;

public class AutoFakerTests
{
    [Fact]
    public Task FakePerson()
    {
        var personFaker = new AutoFaker<Person>()
            .Configure(x => x.WithConventions())
            .UseSeed(4895489);

        var person = personFaker.Generate();

        return Verify(person);
    }
}