namespace FastEndpointsExample.WebApi.Endpoints;

[HttpPost("/api/user/create")]
[AllowAnonymous]
public class CreateUserEndpoint : Endpoint<CreateUserEndpoint.Request, CreateUserEndpoint.Result>
{
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        await SendAsync(new Result
        {
            FullName = $"{req.FirstName} {req.LastName}",
            IsOver18 = req.Age > 18
        }, cancellation: ct);
    }

    public class Request
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
    }

    public class Result
    {
        public string? FullName { get; set; }
        public bool IsOver18 { get; set; }
    }

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("your first name is required!")
                .MinimumLength(5)
                .WithMessage("your name is too short!");

            RuleFor(x => x.Age)
                .NotEmpty()
                .WithMessage("we need your age!")
                .GreaterThan(18)
                .WithMessage("you are not legal yet!");
        }
    }
}