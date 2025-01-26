namespace Configuration.Web.Models;

public class Settings1
{
    public string Value { get; init; }
}

public class Settings1Validator : AbstractValidator<Settings1>
{
    public Settings1Validator()
    {
        RuleFor(x => x.Value).NotNull().NotEmpty();
    }
}