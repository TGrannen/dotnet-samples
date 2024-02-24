using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Polly.CircuitBreaker;
using Polly.Web;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "Polly.Web", Version = "v1" }); });
builder.Services.AddHttpClient("GitHub", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
    client.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
});

builder.Services.AddSingleton<IGitHubService, GitHubService>();
builder.Services.AddTransient<ITestService, TestService>();
builder.Services.Configure<TestOptions>(builder.Configuration.GetSection("TestOptions"));
builder.Services.Configure<HttpRetryStrategyOptions>("ConfiguredHttpOptions",builder.Configuration.GetSection("Resiliency:RetryOptions:Retry"));
builder.Services.AddResilience();

builder.Services
    .AddHttpClient<IFlakyGitHubService, UnReliableGitHubService>((provider, client) =>
    {
        var uriString = builder.Configuration.GetValue<string>("FlakyServerUri");
        client.BaseAddress = new Uri(uriString);
    })
    .AddResilienceHandler("CustomPipeline", static (builder, context) =>
    {
        // Enable reloads whenever the named options change
        context.EnableReloads<HttpRetryStrategyOptions>("ConfiguredHttpOptions");

        // Retrieve the named options
        var retryOptions = context.GetOptions<HttpRetryStrategyOptions>("ConfiguredHttpOptions");
        var logger = context.ServiceProvider.GetRequiredService<ILogger<UnReliableGitHubService>>();
        retryOptions.OnRetry = arguments =>
        {
            logger.LogWarning("Retrying Args {@Arg}", new
            {
                arguments.RetryDelay,
                arguments.Outcome.Exception,
                arguments.Outcome.Result?.StatusCode,
                arguments.AttemptNumber,
            });
            return ValueTask.CompletedTask;
        };

        retryOptions.ShouldHandle = args =>
        {
            if (HttpClientResiliencePredicates.IsTransient(args.Outcome))
            {
                return ValueTask.FromResult(true);
            }

            if (args.Outcome.Exception is BrokenCircuitException)
            {
                return ValueTask.FromResult(false);
            }

            return ValueTask.FromResult(args.Outcome.Exception != null);
        };
        // Add retries using the resolved options
        builder.AddRetry(retryOptions);

        // See: https://www.pollydocs.org/strategies/circuit-breaker.html
        builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
        {
            // Customize and configure the circuit breaker logic.
            SamplingDuration = TimeSpan.FromSeconds(45),
            FailureRatio = 0.5,
            MinimumThroughput = 3,
            BreakDuration = TimeSpan.FromSeconds(30),
            ShouldHandle = static args => ValueTask.FromResult(args is
            {
                Outcome.Result.StatusCode: HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
                or HttpStatusCode.InternalServerError
            }),
            OnOpened = arguments =>
            {
                logger.LogError("Opened Args {@Arg}", new
                {
                    arguments.BreakDuration,
                    arguments.Outcome.Exception,
                    arguments.Outcome.Result?.StatusCode,
                    arguments.IsManual,
                });
                return ValueTask.CompletedTask;
            },
            OnHalfOpened = arguments =>
            {
                logger.LogWarning("Half open");
                return ValueTask.CompletedTask;
            },
            OnClosed = arguments =>
            {
                logger.LogWarning("Closed Args {@Arg}", new
                {
                    arguments.Outcome.Exception,
                    arguments.Outcome.Result?.StatusCode,
                    arguments.IsManual,
                });
                return ValueTask.CompletedTask;
            }
        });

        // See: https://www.pollydocs.org/strategies/timeout.html
        builder.AddTimeout(TimeSpan.FromSeconds(5));
    });

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

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
app.MapControllers();

await app.RunAsync();