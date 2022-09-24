using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Web.Templates;
using Microsoft.AspNetCore.Mvc;

namespace FluentEmail.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TestEmailRenderingController : ControllerBase
{
    private readonly IFluentEmail _fluentEmail;

    public TestEmailRenderingController(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    [HttpPost]
    [Route(nameof(SendWithHtmlRazorFileRenderedBody))]
    public async Task<SendResponse> SendWithHtmlRazorFileRenderedBody()
    {
        return await _fluentEmail
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

        return await _fluentEmail
            .To("bob@email.com", "bob")
            .Subject("Inline Rendering")
            .UsingTemplate(template, new { Name = "Steve", Compliment = "Awesome" })
            .SendAsync();
    }
}