using FluentValidation.Results;

namespace Configuration.Web.Extensions;

public class FluentValidationOptions<T>(string optionsBuilderName, IEnumerable<IValidator<T>> validators) : IValidateOptions<T>
    where T : class
{
    public ValidateOptionsResult Validate(string name, T options)
    {
        // Name mismatches should be skipped
        // Null name is used to configure all named options.
        if (optionsBuilderName != null && name != optionsBuilderName)
        {
            return ValidateOptionsResult.Skip;
        }

        var results = validators?.Select(x => x.Validate(options)).ToArray() ?? [];
        if (results.Length == 0)
        {
            return ValidateOptionsResult.Skip;
        }

        if (results.All(x => x.IsValid))
        {
            return ValidateOptionsResult.Success;
        }

        return ValidateOptionsResult.Fail(results
            .SelectMany(x => x.Errors)
            .Select(r => $"Options validation failed for '{r.PropertyName}' with error: {r.ErrorMessage}."));
    }
}