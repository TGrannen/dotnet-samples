﻿using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Refit;
using Refit.ConsoleApp;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();
var serviceCollection = new ServiceCollection().AddLogging(configure => configure.AddSerilog());

try
{
    serviceCollection
        .AddRefitClient<IYesOrNoApi>()
        .ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://yesno.wtf/"); })
        .AddHttpMessageHandler<HttpLoggingMessageHandler>();
                
    serviceCollection.AddTransient<HttpLoggingMessageHandler>();

    var serviceProvider = serviceCollection.BuildServiceProvider();

    var api = serviceProvider.GetService<IYesOrNoApi>();
    if (api != null)
    {
        Console.WriteLine();
        Console.WriteLine();
        var result = await api.GetResponse();
        Log.Information("Result from API: {@Result}", result);
        Console.WriteLine();
        Console.WriteLine();

        result = await api.ForceResponse("yes");
        Log.Information("Result from API: {@Result}", result);
    }
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}