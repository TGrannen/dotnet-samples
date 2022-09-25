using Polly.Web.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Polly.Web", Version = "v1" }); });
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});

var services = builder.Services;
var registry = services.AddPolicyRegistry();

registry.Add("retry", GetRetryPolicy());
registry.Add("circuit", GetCircuitBreakerPolicy());

services.AddSingleton<IGitHubService, GitHubService>();

services
    .AddHttpClient<IFlakyGitHubService, UnReliableGitHubService>((provider, client) =>
    {
        var uriString = builder.Configuration.GetValue<string>("FlakyServerUri");
        client.BaseAddress = new Uri(uriString);
    })
    .AddPollyContextLoggingNoOpPolicy<UnReliableGitHubService>()
    .AddPolicyHandlerFromRegistry("circuit")
    .AddPolicyHandlerFromRegistry("retry")
    ;

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polly.Web v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

await app.RunAsync();

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt) * 100),
            onRetry: (exception, duration, retryCount, context) =>
            {
                context.GetLogger()
                    .LogWarning("Retry Number: {RetryCount}  Waiting: {Duration:#}ms, due to: {Message}",
                        retryCount,
                        duration.TotalMilliseconds,
                        exception.Exception?.Message ?? exception.Result.ToString());
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .Or<Exception>()
        .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
            onBreak: (result, state, duration, context) =>
            {
                context.GetLogger().LogWarning("CircuitBreaker PreviousState:{PreviousState} State:{State} Duration {Duration:#}", state,
                    CircuitState.Open, duration.TotalSeconds);
            },
            onReset: context => { context.GetLogger().LogWarning("CircuitBreaker State:{State}", CircuitState.Closed); },
            () => { });
}