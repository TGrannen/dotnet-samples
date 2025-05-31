using MinimalAPIsExample.WebAPI.Endpoints.Posts;
using MinimalAPIsExample.WebAPI.Endpoints.WeatherForecasts;

namespace MinimalAPIsExample.WebAPI.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetWeatherForecasts();
        app.MapCreatePost();
        return app;
    }
}