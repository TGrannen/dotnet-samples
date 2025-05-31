using MinimalAPIsExample.WebAPI.Validation;

namespace MinimalAPIsExample.WebAPI.Endpoints.Posts;

public static class CreatePostEndpoint
{
    public static IEndpointRouteBuilder MapCreatePost(this IEndpointRouteBuilder app)
    {
        app.MapPost("/posts", (CreatePostRequest request) =>
            {
                var post = new Post
                {
                    Title = request.Title.Trim(),
                    Content = request.Content.Trim()
                };

                return TypedResults.Ok(post.Id);
            })
            .WithName("CreatePost")
            .WithRequestValidation<CreatePostRequest>();

        return app;
    }

    public record CreatePostRequest(string Title, string Content);

    public class CreatePostValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostValidator()
        {
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}