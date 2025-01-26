// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using Humanizer;

namespace ConsoleApp.Commands;

public class ListFilesCommand : Command<ListFilesCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [Description("Path to search. Defaults to current directory.")]
        [CommandArgument(0, "[searchPath]")]
        public string? SearchPath { get; init; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var searchOptions = new EnumerationOptions
        {
            AttributesToSkip = FileAttributes.System
        };

        var searchPath = settings.SearchPath ?? Directory.GetCurrentDirectory();
        var directoryInfo = new DirectoryInfo(searchPath);

        var table = new Table();
        table.AddColumn(new TableColumn("Name") { Alignment = Justify.Right });
        table.AddColumn("Size");
        table.AddColumn("Last Access Time");
        foreach (var directory in directoryInfo.GetDirectories())
        {
            table.AddRow(directory.Name, Emoji.Known.FileFolder);
        }

        foreach (var file in directoryInfo.GetFiles("*.*", searchOptions))
        {
            table.AddRow(file.Name, file.Length.Bytes().Humanize(), file.LastAccessTime.Humanize());
        }

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"File contents for [green]{directoryInfo.FullName}[/]");
        AnsiConsole.WriteLine();
        AnsiConsole.Write(table);

        return 0;
    }
}