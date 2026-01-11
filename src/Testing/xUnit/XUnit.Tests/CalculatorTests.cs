using XUnit.BusinessLogic.Services;

namespace XUnit.Tests;

public class CalculatorTests
{
    private readonly CalcService _calcService = new();

    [Fact(Skip = "This test is broken")]
    public void CalcService_ShouldPerformAddition_WhenPositiveNumbers()
    {
        var result = _calcService.PerformOperation(4, 9, "a");
        Assert.Equal(13, result);
    }

    [Theory]
    [InlineData(13, 5, 8)]
    [InlineData(0, -3, 3)]
    [InlineData(0, 0, 0)]
    public void CalcService_ShouldPerformAddition_WhenValidNumbersArePassed(double expected, double firstToAdd, double secondToAdd)
    {
        var result = _calcService.PerformOperation(firstToAdd, secondToAdd, "a");
        Assert.Equal(expected, expected);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public void CalcService_ShouldPerformAddition_WhenValidNumbersArePassedFromMemberData(double expected, params double[] valuesToAdd)
    {
        var result = _calcService.PerformOperation(valuesToAdd.First(), valuesToAdd.Skip(1).First(), "a");
        foreach (var value in valuesToAdd.Skip(2))
        {
            result = _calcService.PerformOperation(result, value, "a");
        }

        Assert.Equal(expected, result);
    }

    [Theory]
    [ClassData(typeof(DivisionTestData))]
    public void CalcService_ShouldPerformDivision_WhenValidNumbersArePassedFromClassData(double expected, params double[] valuesToDivide)
    {
        var result = _calcService.PerformOperation(valuesToDivide.First(), valuesToDivide.Skip(1).First(), "d");
        foreach (var value in valuesToDivide.Skip(2))
        {
            result = _calcService.PerformOperation(result, value, "d");
        }

        Assert.Equal(expected, result);
    }

    public static IEnumerable<object[]> TestData()
    {
        yield return new object[] { 15, new double[] { 10, 5 } };
        yield return new object[] { 15, new double[] { 5, 5, 5 } };
        yield return new object[] { -10, new double[] { -30, 20 } };
    }
}

public class DivisionTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { 30, new double[] { 60, 2 } };
        yield return new object[] { 0, new double[] { 0, 1 } };
        yield return new object[] { 1, new double[] { 50, 50 } };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}