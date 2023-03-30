// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ConsoleApp.Commands;

public class HelloCommand : Command<HelloCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "[Name]")] public string? Name { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        AnsiConsole.MarkupLine($"Hello, [blue]{settings.Name}[/]");
        return 0;
    }
}