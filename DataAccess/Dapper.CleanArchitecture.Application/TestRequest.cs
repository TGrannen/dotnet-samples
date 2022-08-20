namespace Dapper.CleanArchitecture.Application;

public class TestRequest : IRequest<TestRequestVm>
{
}

public class TestRequestHandler : IRequestHandler<TestRequest, TestRequestVm>
{
    private readonly IDbContext _context;

    public TestRequestHandler(IDbContext context)
    {
        _context = context;
    }
    public async Task<TestRequestVm> Handle(TestRequest request, CancellationToken cancellationToken)
    {
       await Task.Delay(500, cancellationToken);

       return new TestRequestVm();
    }
}

public class TestRequestVm
{
}