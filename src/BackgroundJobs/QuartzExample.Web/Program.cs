using Microsoft.OpenApi;
using Quartz;
using QuartzExample.Web.Extensions;
using QuartzExample.Web.Jobs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuartzExample.Web", Version = "v1" }); });

builder.Services.ConfigureQuartz(builder.Configuration);

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QuartzExample.Web v1"));
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetService<ILogger<Program>>();
    var scheduler = app.Services.GetService<IScheduler>();

    scheduler.ConfigureJobWithCronSchedule<ExampleJob1>(logger, app.Configuration, "BackgroundTasks:ExampleJob1CronExpression");
});

await app.RunAsync();