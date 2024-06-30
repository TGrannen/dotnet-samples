using AwsMessagingTest.BackgroundServices;
using AwsMessagingTest.Messages;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); });
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddHostedService<Worker>();

// Register the AWS Message Processing Framework for .NET
builder.Services.AddAWSMessageBus(messageBusBuilder =>
{
    var chatUrl = builder.Configuration.GetValue("Queues:ChatUrl", "Not-set")!;
    var orderInfoUrl = builder.Configuration.GetValue("Queues:OrderInfoUrl", "Not-set")!;
    // Register that you'll publish messages of type ChatMessage to an existing queue
    messageBusBuilder.AddSQSPublisher<ChatMessage>(chatUrl);
    messageBusBuilder.AddSQSPublisher<OrderInfo>(orderInfoUrl);

    // Register an SQS Queue that the framework will poll for messages
    messageBusBuilder.AddSQSPoller(chatUrl, options =>
    {
        // The maximum number of messages from this queue that the framework will process concurrently on this client
        options.MaxNumberOfConcurrentMessages = 10;

        // The duration each call to SQS will wait for new messages
        options.WaitTimeSeconds = 20;
    });

    messageBusBuilder.AddSQSPoller(orderInfoUrl);

    // Register all IMessageHandler implementations with the message type they should process. 
    // Here messages that match our ChatMessage .NET type will be handled by our ChatMessageHandler
    messageBusBuilder.AddMessageHandler<ChatMessageHandler, ChatMessage>();
    messageBusBuilder.AddMessageHandler<OrderInfoHandler, OrderInfo>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();
app.MapControllers();

app.Run();