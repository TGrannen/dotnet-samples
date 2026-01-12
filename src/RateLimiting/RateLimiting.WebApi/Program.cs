using AspNetCoreRateLimit;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "RateLimiting.WebApi", Version = "v1" }); });

// needed to store rate limit counters and ip rules
builder.Services.AddMemoryCache();

//load general configuration from appsettings.json
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

//load ip rules from appsettings.json
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

//load general configuration from appsettings.json
builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("ClientRateLimiting"));

//load client rules from appsettings.json
builder.Services.Configure<ClientRateLimitPolicies>(builder.Configuration.GetSection("ClientRateLimitPolicies"));

// inject counter and rules stores
builder.Services.AddInMemoryRateLimiting();
//builder.Services.AddDistributedRateLimiting<AsyncKeyLockProcessingStrategy>();
//builder.Services.AddDistributedRateLimiting<RedisProcessingStrategy>();
//builder.Services.AddRedisRateLimiting();

// configuration (resolvers, counter key builders)
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

var app = builder.Build();
app.UseSerilogRequestLogging();

// app.UseIpRateLimiting();
app.UseClientRateLimiting();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RateLimiting.WebApi v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();