using ApiService.Api.Common.Web.Versioning;

namespace ApiService.Api.Tests.Features.Customers;

public record CreateCustomerRequest(string Name, string Email);

public record CustomerResponse(Guid Id, string Name, string Email);

public record ListCustomersQuery(int Skip, int Take);

[Headers(SupportedApiVersions.V1RefitHeaderLine)]
public interface ICustomersApi
{
    [Post("/api/customers")]
    Task<TResponse> CreateAsync<TResponse>([Body] object request);

    [Get("/api/customers")]
    Task<List<CustomerResponse>> ListAsync([Query] ListCustomersQuery query);

    [Get("/api/customers/{id}")]
    Task<TResponse> GetAsync<TResponse>(Guid id);
}
