using ConsoleApp.Commands;

var app = new CommandApp();
app.Configure(config =>
{
    config.AddCommand<HelloCommand>("hello")
        .WithAlias("hola")
        .WithDescription("Say hello")
        .WithExample(["hello", "Phil"])
        .WithExample(["hello", "Phil", "--count", "4"]);

    config.AddCommand<FileSizeCommand>("size")
        .WithAlias("file-size")
        .WithDescription("Gets the file size for a directory.")
        .WithExample(["size"])
        .WithExample(["size", "c:\\windows"])
        .WithExample(["size", "c:\\windows", "--pattern", "*.dll"])
        ;

    config.AddCommand<ListFilesCommand>("list")
        .WithDescription("Prints the files within a directory.")
        .WithExample(["list"])
        .WithExample(["list", "c:\\windows"])
        ;
});

return app.Run(args);