using AwesomeAssertions;

namespace Mocking.AutoBogus;

public class StableAutoFakerTests
{
    [Fact]
    public Task FakePerson()
    {
        var person = StableAutoFaker.Generate<Person>();

        return Verify(person);
    }

    [Fact]
    public void TestRandomizer()
    {
        var faker1 = StableAutoFaker.NewFaker("User.FirstName");
        var faker2 = StableAutoFaker.NewFaker("User.FirstName");

        faker1.Name.FirstName().Should().Be(faker2.Name.FirstName());
    }
}