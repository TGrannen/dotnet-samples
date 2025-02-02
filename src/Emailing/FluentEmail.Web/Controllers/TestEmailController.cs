namespace FluentEmail.Web.Controllers;

[ApiController]
[Route("api")]
public class TestEmailController(IFluentEmail fluentEmail, IOptions<EmailConfig> options) : ControllerBase
{
    [HttpPost]
    [Route(nameof(SendWithHtmlRazorFileRenderedBody))]
    public async Task<SendResponse> SendWithHtmlRazorFileRenderedBody()
    {
        return await fluentEmail
            .To("bob@email.com", "bob")
            .Subject("Fancy Rendered Email")
            .UsingTemplateFromFile("Templates/FancyEmailTemplate.cshtml", new FancyEmailTemplateModel { Name = "James", Value = 560 })
            .SendAsync();
    }

    [HttpPost]
    [Route(nameof(SendWithInlineRenderedBody))]
    public async Task<SendResponse> SendWithInlineRenderedBody()
    {
        var template = "Dear @Model.Name, You are totally @Model.Compliment.";

        return await fluentEmail
            .To("bob@email.com", "bob")
            .Subject("Inline Rendering")
            .UsingTemplate(template, new { Name = "Steve", Compliment = "Awesome" })
            .SendAsync();
    }

    [HttpPost]
    [Route(nameof(SendWithImageAttachment))]
    public async Task<SendResponse> SendWithImageAttachment()
    {
        return await fluentEmail
            .To("bob@email.com", "bob")
            .Subject("With Attachment")
            .Body("yo bob, long time no see!")
            .AttachFromFilename("Content/meme.png")
            .SendAsync();
    }

    [HttpPost]
    [Route(nameof(PostToSendGrid))]
    public async Task<SendResponse> PostToSendGrid()
    {
        var sendGridConfig = options.Value.SendGrid;
        var sender = new SendGridSender(sendGridConfig.APIKey, sendGridConfig.UseSandbox);

        var email = Email
            .From("john@email.com")
            .To("bob@email.com", "bob")
            .Subject("hows it going bob")
            .Body("yo bob, long time no see!");

        return await sender.SendAsync(email);
    }

    [HttpPost]
    [Route(nameof(PostToSMTP))]
    public async Task<SendResponse> PostToSMTP()
    {
        return await fluentEmail
            //.From("john@email.com") // Set by default in DI or can be overriden with SetFrom(...)
            .To("bob@email.com", "bob")
            .Subject("hows it going bob")
            .Body("yo bob, long time no see!")
            .SendAsync();
    }
}