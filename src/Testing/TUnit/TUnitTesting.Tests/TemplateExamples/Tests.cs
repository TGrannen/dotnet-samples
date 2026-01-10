using TUnitTesting.Tests.TemplateExamples.Data;

namespace TUnitTesting.Tests.TemplateExamples;

public class Tests
{
    [Test]
    public void Basic()
    {
        Console.WriteLine("This is a basic test");
    }

    [Test]
    [Arguments(1, 2, 3, DisplayName = "One plus two equals three")]
    [Arguments(2, 3, 5, DisplayName = "Adding $a + $b = $expected")]
    [Arguments(5, 1, 6, Skip = "Test skipping for now")]
    [Arguments(100, 50, 150, Categories = ["LargeNumbers", "Performance"])]
    public async Task DataDrivenArguments(int a, int b, int expected)
    {
        Console.WriteLine("This one can accept arguments from an attribute");

        var result = a + b;

        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    [MethodDataSource(nameof(DataSource))]
    public async Task MethodDataSource(int a, int b, int c)
    {
        Console.WriteLine("This one can accept arguments from a method");

        var result = a + b;

        await Assert.That(result).IsEqualTo(c);
    }

    [Test]
    [MethodDataSource(nameof(ObjectDataSource))]
    public async Task MethodObjectDataSource(AdditionTestData data)
    {
        Console.WriteLine("This one can accept arguments from a method");

        var result = data.Value1 + data.Value2;

        await Assert.That(result).IsEqualTo(data.ExpectedResult);
    }

    [Test]
    [ClassDataSource<DataClass>]
    [ClassDataSource<DataClass>(Shared = SharedType.PerClass)]
    [ClassDataSource<DataClass>(Shared = SharedType.PerAssembly)]
    [ClassDataSource<DataClass>(Shared = SharedType.PerTestSession)]
    public void ClassDataSource(DataClass dataClass)
    {
        Console.WriteLine("This test can accept a class, which can also be pre-initialised before being injected in");

        Console.WriteLine("These can also be shared among other tests, or new'd up each time, by using the `Shared` property on the attribute");
    }

    public record AdditionTestData(int Value1, int Value2, int ExpectedResult);

    public static IEnumerable<Func<AdditionTestData>> ObjectDataSource()
    {
        yield return () => new AdditionTestData(1, 1, 2);
        yield return () => new AdditionTestData(2, 1, 3);
        yield return () => new AdditionTestData(3, 1, 4);
    }

    public static IEnumerable<Func<(int a, int b, int c)>> DataSource()
    {
        yield return () => (1, 1, 2);
        yield return () => (2, 1, 3);
        yield return () => (3, 1, 4);
    }
}