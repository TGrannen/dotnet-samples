using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentEmail.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TestEmailAttachmentsController : ControllerBase
{
    private readonly IFluentEmail _fluentEmail;

    public TestEmailAttachmentsController(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    [HttpPost]
    [Route(nameof(SendWithImageAttachment))]
    public async Task<SendResponse> SendWithImageAttachment()
    {
        return await _fluentEmail
            .To("bob@email.com", "bob")
            .Subject("With Attachment")
            .Body("yo bob, long time no see!")
            .AttachFromFilename("Content/meme.png")
            .SendAsync();
    }
}