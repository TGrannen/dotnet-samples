using FeatureFlags.LaunchDarkly.WebAPI.Services;
using FeatureFlags.Library.LaunchDarkly;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FeatureFlags.LaunchDarkly.WebAPI", Version = "v1" });
});
builder.Services.AddLaunchDarkly(ldConfig => { ldConfig.SdkKey = builder.Configuration.GetValue<string>("Feature:LaunchDarkly:SdkKey"); });

builder.Services.AddTransient<IUserService, RandomUserService>();
builder.Services.AddTransient<LibraryService>();
builder.Services.AddTransient<LaunchDarklyDirectService>();

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FeatureFlags.LaunchDarkly.WebAPI v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.UseLaunchDarkly(app.Lifetime);

await app.RunAsync();