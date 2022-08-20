using Dapper.CleanArchitecture.Application.Employees.Queries;
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


    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var vm = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(vm);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> GetById(int employeeNumber)
    {
        var vm = await _mediator.Send(new GetEmployeeByIdQuery { EmployeeNumber = employeeNumber });
        return Ok(vm);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(int employee)
    {
        var vm = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(vm);
    }


    [HttpPatch("Update")]
    public async Task<IActionResult> Update(int employee)
    {
        var vm = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(vm);
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> DeleteById(int employeeNumber)
    {
        var vm = await _mediator.Send(new GetAllEmployeesQuery());
        return Ok(vm);
    }
}