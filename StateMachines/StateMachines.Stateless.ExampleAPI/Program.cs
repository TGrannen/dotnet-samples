using Microsoft.OpenApi.Models;
using Serilog;
using StateMachines.Stateless.ExampleAPI.DynamicSM;
using StateMachines.Stateless.ExampleAPI.PhoneCall;
using StateMachines.Stateless.ExampleAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StateMachines.Stateless.ExampleAPI", Version = "v1" });
    var filePath = Path.Combine(AppContext.BaseDirectory, "StateMachines.Stateless.ExampleAPI.xml");
    c.IncludeXmlComments(filePath);
});
builder.Services.AddSingleton<PhoneCallSm>();
builder.Services.AddSingleton<StateMachineExporter>();
builder.Services.AddSingleton<DynamicStateMachineService>();

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });

var app = builder.Build();
app.UseSerilogRequestLogging();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StateMachines.Stateless.ExampleAPI v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
await app.RunAsync();