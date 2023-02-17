namespace Configuration.Web.Extensions;

public class FluentValidationOptions<T> : IValidateOptions<T> where T : class
{
    private readonly string _name;
    private readonly IEnumerable<IValidator<T>> _validators;

    public FluentValidationOptions(string optionsBuilderName, IEnumerable<IValidator<T>> validators)
    {
        _name = optionsBuilderName;
        _validators = validators;
    }

    public ValidateOptionsResult Validate(string name, T options)
    {
        // Null name is used to configure all named options.
        if (_name != null && name != _name)
        {
            return ValidateOptionsResult.Skip;
        }

        var results = _validators.Select(x => x.Validate(options)).ToArray();
        if (results.All(x => x.IsValid))
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(results
            .SelectMany(x => x.Errors)
            .Select(r => $"Options validation failed for '{r.PropertyName}' with error: {r.ErrorMessage}."));
    }
}