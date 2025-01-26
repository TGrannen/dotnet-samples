using ConsoleApp.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<HelloCommand>("hello")
        .WithAlias("hola")
        .WithDescription("Say hello")
        .WithExample(new[] { "hello", "Phil" })
        .WithExample(new[] { "hello", "Phil", "--count", "4" });

    config.AddCommand<FileSizeCommand>("size")
        .WithAlias("file-size")
        .WithDescription("Gets the file size for a directory.")
        .WithExample(new[] { "size" })
        .WithExample(new[] { "size", "c:\\windows" })
        .WithExample(new[] { "size", "c:\\windows", "--pattern", "*.dll" })
        ;

    config.AddCommand<ListFilesCommand>("list")
        .WithDescription("Prints the files within a directory.")
        .WithExample(new[] { "list" })
        .WithExample(new[] { "list", "c:\\windows" })
        ;
});

return app.Run(args);