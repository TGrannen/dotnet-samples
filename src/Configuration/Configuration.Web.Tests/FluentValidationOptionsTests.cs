namespace Configuration.Web.Tests;

public class FluentValidationOptionsTests
{
    [Fact]
    public void Validate_ReturnsSuccess_WhenOptionsAreValid()
    {
        var result = Validate(new TestOptions { Value = "Valid Value" });

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ReturnsFailed_WhenOptionsAreInvalid()
    {
        var result = Validate(new TestOptions { Value = "NOPE" });

        result.Failed.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ReturnsSkipped_WhenNameDoesNotMatch()
    {
        var result = Validate(new TestOptions(), "TEST", "OTHER");

        result.Skipped.ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TEST")]
    public void Validate_ReturnsSkipped_WhenNoValidatorsProvided(string name)
    {
        var validator = new FluentValidationOptions<TestOptions>(name, []);
        var result = validator.Validate(name, new TestOptions());

        result.Skipped.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ReturnsSkipped_WhenValidatorsIsNull()
    {
        var validator = new FluentValidationOptions<TestOptions>("TEST", null);
        var result = validator.Validate("TEST", new TestOptions());

        result.Skipped.ShouldBeTrue();
    }

    private static ValidateOptionsResult Validate(TestOptions options, string builderName = "TEST", string validateName = "TEST")
    {
        var validator = new FluentValidationOptions<TestOptions>(builderName, [new TestOptionsValidator()]);
        return validator.Validate(validateName, options);
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