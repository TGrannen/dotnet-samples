using NUnit.BusinessLogic.Services;

namespace NUnit.Tests;

[TestFixture(50, 100.00)]
[TestFixture(-50, 20)]
public class CalcServiceTestsWithFixture
{
    private readonly double _first;
    private readonly double _second;

    public CalcServiceTestsWithFixture(double first, double second)
    {
        _first = first;
        _second = second;
    }

    private CalcService _calcService;

    [SetUp]
    public void Setup()
    {
        _calcService = new CalcService();
    }

    [TestCase]
    public void AddMethod()
    {
        var result = _calcService.PerformOperation(_first, _second, "a");
        Assert.That(result, Is.EqualTo(_first + _second));
    }

    [TestCase]
    public void SubtractMethod()
    {
        var result = _calcService.PerformOperation(_first, _second, "s");
        Assert.That(result, Is.EqualTo(_first - _second));
    }

    [TestCase]
    public void MultiplyMethod()
    {
        var result = _calcService.PerformOperation(_first, _second, "m");
        Assert.That(result, Is.EqualTo(_first * _second));
    }

    [TestCase]
    public void DivideMethod()
    {
        var result = _calcService.PerformOperation(_first, _second, "d");
        Assert.That(result, Is.EqualTo(_first / _second));
    }
}