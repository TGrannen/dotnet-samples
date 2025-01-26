using RealTime.SignalRBackplane.Server.Hub;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(builder.Configuration); });
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var addSignalR = builder.Services.AddSignalR();
if (!builder.Configuration.GetValue<bool>("Redis:IgnoreBackplane"))
{
    addSignalR
        .AddStackExchangeRedis(builder.Configuration.GetValue<string>("Redis:ConnectionString"),
            options => { options.Configuration.ChannelPrefix = builder.Configuration.GetValue<string>("Redis:ChannelPrefix"); });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapHub<ChatHub>("/chathub");

app.Run();