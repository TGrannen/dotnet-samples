namespace SnapshotTesting.VerifyTests;

public class SampleDataTests
{
    [Fact]
    public Task Test1()
    {
        var value = new
        {
            Id = Guid.NewGuid(),
            GivenNames = "John",
            FamilyName = "Smith",
            Spouse = "Jill",
            Children = new List<string>
            {
                "Sam",
                "Mary"
            },
            Address = new
            {
                Street = "4 Puddle Lane",
                Country = "USA"
            }
        };

        return Verify(value);
    }
}