namespace MinimalAPIsExample.WebAPI.Endpoints.Posts;

public class Post
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public required string Title { get; set; }
    public required string Content { get; set; }
}