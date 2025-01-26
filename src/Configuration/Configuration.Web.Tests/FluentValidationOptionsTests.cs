using Configuration.Web.Extensions;

namespace Configuration.Web.Tests;

public class FluentValidationOptionsTests
{
    [Fact]
    public void Validate_ShouldReturnSuccess_WhenValidationPassed()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", new[] { new TestOptionsValidator() });
        var result = options.Validate("TEST", new TestOptions { Value = "My Value" });
        result.Should().BeEquivalentTo(new
        {
            Skipped = false,
            Failed = false,
            Succeeded = true,
        });
    }

    [Fact]
    public void Validate_ShouldReturnFailed_WhenValidationFailed()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", new[] { new TestOptionsValidator() });
        var result = options.Validate("TEST", new TestOptions { Value = "NOPE" });
        result.Should().BeEquivalentTo(new
        {
            Skipped = false,
            Failed = true,
            Succeeded = false,
        });
    }

    [Theory]
    [InlineData("TEST", "TEST2")]
    public void Validate_ShouldReturnSkipped_WhenNamesDoNotMatch(string builderName, string validateName)
    {
        var options = new FluentValidationOptions<TestOptions>(builderName, new[] { new TestOptionsValidator() });
        var result = options.Validate(validateName, new TestOptions());
        result.Should().BeEquivalentTo(new
        {
            Skipped = true,
            Failed = false,
            Succeeded = false,
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TEST")]
    public void Validate_ShouldReturnSkipped_WhenNoValidatorsInjected(string name)
    {
        var options = new FluentValidationOptions<TestOptions>(name, new List<IValidator<TestOptions>>());
        var result = options.Validate(name, new TestOptions());
        result.Should().BeEquivalentTo(new
        {
            Skipped = true,
            Failed = false,
            Succeeded = false,
        });
    }

    [Fact]
    public void Validate_ShouldReturnSkipped_WhenInjectedListIsNull()
    {
        var options = new FluentValidationOptions<TestOptions>("TEST", null);
        var result = options.Validate("TEST", new TestOptions());
        result.Should().BeEquivalentTo(new
        {
            Skipped = true,
            Failed = false,
            Succeeded = false,
        });
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