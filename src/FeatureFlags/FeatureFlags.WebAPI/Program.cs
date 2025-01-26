using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "FeatureFlags.WebAPI", Version = "v1" }); });

builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PercentageFilter>()
    .AddFeatureFilter<TimeWindowFilter>()
    .AddFeatureFilter<CustomEndpointFilter>()
    .AddFeatureFilter<CustomContextualFilter>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFeatureService, FeatureService>();

var connection = builder.Configuration.GetConnectionString("AppConfig");
builder.Services.AddAzureAppConfiguration();
builder.Configuration.AddAzureAppConfiguration(options => { options.Connect(connection).UseFeatureFlags(); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FeatureFlags.WebAPI v1"));
}

app.UseHttpsRedirection();
app.UseAzureAppConfiguration();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();