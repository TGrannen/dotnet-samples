using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.SendGrid;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FluentEmail.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly IConfiguration _configuration;

        public TestEmailController(IFluentEmail fluentEmail, IConfiguration configuration)
        {
            _fluentEmail = fluentEmail;
            _configuration = configuration;
        }

        [HttpPost]
        [Route(nameof(PostToSendGrid))]
        public async Task<SendResponse> PostToSendGrid()
        {
            var apiKey = _configuration.GetValue("EmailConfig:SendGrid:APIKey", "dummy-api-key");
            var sandBoxMode = _configuration.GetValue("EmailConfig:SendGrid:UseSandbox", true);

            var sender = new SendGridSender(apiKey, sandBoxMode);

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
            return await _fluentEmail
                //.From("john@email.com") // Set by default in DI or can be overriden with SetFrom(...)
                .To("bob@email.com", "bob")
                .Subject("hows it going bob")
                .Body("yo bob, long time no see!")
                .SendAsync();
        }
    }
}