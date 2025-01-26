using NUnit.BusinessLogic.Services;

namespace NUnit.Tests;

public class CalcServiceTests
{
    private CalcService _calcService;

    [SetUp]
    public void Setup()
    {
        _calcService = new CalcService();
    }

    [Test]
    public void CalcService_ShouldPerformAddition_WhenPositiveNumbers()
    {
        var result = _calcService.PerformOperation(4, 9, "a");
        Assert.AreEqual(13, result);
    }

    [Theory]
    [TestCase(13, 5, 8)]
    [TestCase(0, -3, 3)]
    [TestCase(0, 0, 0)]
    public void CalcService_ShouldPerformAddition_WhenValidNumbersArePassed(double expected, double firstToAdd, double secondToAdd)
    {
        var result = _calcService.PerformOperation(firstToAdd, secondToAdd, "a");
        Assert.AreEqual(expected, expected);
    }

    [Theory]
    [TestCaseSource(nameof(TestData))]
    public void CalcService_ShouldPerformAddition_WhenValidNumbersArePassedFromMemberData(double expected, params double[] valuesToAdd)
    {
        var result = _calcService.PerformOperation(valuesToAdd.First(), valuesToAdd.Skip(1).First(), "a");
        foreach (var value in valuesToAdd.Skip(2))
        {
            result = _calcService.PerformOperation(result, value, "a");
        }

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 15, new double[] { 10, 5 } };
        yield return new object[] { 15, new double[] { 5, 5, 5 } };
        yield return new object[] { -10, new double[] { -30, 20 } };
    }
}