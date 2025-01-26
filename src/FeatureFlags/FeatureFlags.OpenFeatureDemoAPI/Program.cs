using FeatureFlags.OpenFeatureDemoAPI;
using OpenFeature.Contrib.Providers.Flagd;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddKeyedSingleton<InMemoryProvider>("configuration", (sp, k) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var flags = new Dictionary<string, Flag>
    {
        { "ShowWeather", configuration.GenerateConfigurationBoolFlag("FeatureFlags:ShowWeather") },
        { "ShouldBeSuperCold", configuration.GenerateConfigurationBoolFlag("FeatureFlags:ShouldBeSuperCold") },
        { "ShouldHaveOnlyOne", configuration.GenerateConfigurationBoolFlag("FeatureFlags:ShouldHaveOnlyOne") },
        {
            "AllowForMinNumber",
            configuration.GenerateConfigurationFlag<int>("FeatureFlags:MinNumber",
                (context, value) => context.GetValue("RandomNumber").AsInteger > value)
        },
    };

    return new InMemoryProvider(flags);
});
builder.Services.AddScoped<IFeatureClient>(sp => sp.GetRequiredService<Api>().GetClient());
builder.Services.AddKeyedScoped<IFeatureClient>("flag-d", (sp, k) => sp.GetRequiredService<Api>().GetClient(k.ToString()));
builder.Services.AddKeyedScoped<IFeatureClient>("configuration", (sp, k) => sp.GetRequiredService<Api>().GetClient(k.ToString()));

builder.Services.AddSingleton<FlagdProvider>(_ => new FlagdProvider(new FlagdConfigBuilder()
    .WithHost("localhost")
    .WithPort(8013)
    .Build()));

builder.Services.AddOpenFeature(featureBuilder =>
{
    featureBuilder
        .AddHostedFeatureLifecycle()
        .AddContext((contextBuilder, serviceProvider) =>
        {
            var context = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (context != null) contextBuilder.Set("Path", context.Request.Path);
        })
        .AddProvider("configuration", (sp, _) => sp.GetRequiredKeyedService<InMemoryProvider>("configuration"))
        .AddProvider("flag-d", (sp, _) => sp.GetRequiredService<FlagdProvider>())
        .AddPolicyName(options =>
        {
            // Custom logic to select a default provider
            options.DefaultNameSelector = serviceProvider => "configuration";
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();