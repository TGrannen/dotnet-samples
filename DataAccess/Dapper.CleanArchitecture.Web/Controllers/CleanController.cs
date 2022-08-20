using Dapper.CleanArchitecture.Application;
using MediatR;

namespace Dapper.CleanArchitecture.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CleanController : ControllerBase
{
    private readonly IMediator _mediator;

    public CleanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        var vm = await _mediator.Send(new TestRequest());
        return Ok(vm);
    }
}