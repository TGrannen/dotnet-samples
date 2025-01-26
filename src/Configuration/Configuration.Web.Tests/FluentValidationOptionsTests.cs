using Configuration.Web.Extensions;

namespace Configuration.Web.Tests;

public class FluentValidationOptionsTests
{
    [Fact]
    public void Validate_ShouldReturnSuccess_WhenValidationPassed()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", new[] { new TestOptionsValidator() });
        var result = options.Validate("TEST", new TestOptions { Value = "My Value" });
        result.Skipped.ShouldBe(false);
        result.Failed.ShouldBe(false);
        result.Succeeded.ShouldBe(true);
    }

    [Fact]
    public void Validate_ShouldReturnFailed_WhenValidationFailed()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", new[] { new TestOptionsValidator() });
        var result = options.Validate("TEST", new TestOptions { Value = "NOPE" });

        result.Skipped.ShouldBe(false);
        result.Failed.ShouldBe(true);
        result.Succeeded.ShouldBe(false);
    }

    [Theory]
    [InlineData("TEST", "TEST2")]
    public void Validate_ShouldReturnSkipped_WhenNamesDoNotMatch(string builderName, string validateName)
    {
        var options = new FluentValidationOptions<TestOptions>(builderName, new[] { new TestOptionsValidator() });
        var result = options.Validate(validateName, new TestOptions());

        result.Skipped.ShouldBe(true);
        result.Failed.ShouldBe(false);
        result.Succeeded.ShouldBe(false);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TEST")]
    public void Validate_ShouldReturnSkipped_WhenNoValidatorsInjected(string name)
    {
        var options = new FluentValidationOptions<TestOptions>(name, new List<IValidator<TestOptions>>());
        var result = options.Validate(name, new TestOptions());

        result.Skipped.ShouldBe(true);
        result.Failed.ShouldBe(false);
        result.Succeeded.ShouldBe(false);
    }

    [Fact]
    public void Validate_ShouldReturnSkipped_WhenInjectedListIsNull()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", null);
        var result = options.Validate("TEST", new TestOptions());

        result.Skipped.ShouldBe(true);
        result.Failed.ShouldBe(false);
        result.Succeeded.ShouldBe(false);
    }
}

public class TestOptions
{
    public string? Value { get; init; }
}

public class TestOptionsValidator : AbstractValidator<TestOptions>
{
    public TestOptionsValidator()
    {
        RuleFor(x => x.Value).MinimumLength(5);
    }
}