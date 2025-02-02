using System.Net.Mail;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var emailConfig = builder.Configuration.GetSection("EmailConfig").Get<EmailConfig>();
builder.Services
    .AddFluentEmail(emailConfig.FromAddress)
    .AddRazorRenderer(Path.Combine(builder.Environment.ContentRootPath, "Templates/"))
    //.AddSendGridSender(emailConfig.SendGrid.APIKey, emailConfig.SendGrid.UseSandbox);
    .AddSmtpSender(new SmtpClient(emailConfig.SMTPConfig.Server, emailConfig.SMTPConfig.Port));

var app = builder.Build();
app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();