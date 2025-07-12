namespace TUnitTesting.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController(ApplicationDbContext context) : ControllerBase
{
    // GET: api/Posts
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PostResponseDto>>> GetPosts()
    {
        var posts = await context.Posts
            .Include(p => p.Comments)
            .Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Comments = p.Comments.Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId
                }).ToList()
            })
            .ToListAsync();
        return Ok(posts);
    }

    // GET: api/Posts/5
    [HttpGet("{id}")]
    public async Task<ActionResult<PostResponseDto>> GetPost(int id)
    {
        var post = await context.Posts
            .Include(p => p.Comments)
            .Where(p => p.Id == id)
            .Select(p => new PostResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                Comments = p.Comments.Select(c => new CommentResponseDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    PostId = c.PostId
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    // POST: api/Posts
    // This endpoint still takes a PostDto for input
    [HttpPost]
    public async Task<ActionResult<PostResponseDto>> CreatePost(PostDto postDto)
    {
        var post = new Post
        {
            Title = postDto.Title,
            Content = postDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        context.Posts.Add(post);
        await context.SaveChangesAsync();

        // Return the newly created post as a DTO
        var responseDto = new PostResponseDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            Comments = new List<CommentResponseDto>() // New post has no comments initially
        };

        return CreatedAtAction(nameof(GetPost), new { id = post.Id }, responseDto);
    }

    // PUT: api/Posts/5
    // This endpoint still takes a PostDto for input
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, PostDto postDto)
    {
        var existingPost = await context.Posts.FindAsync(id);
        if (existingPost == null)
        {
            return NotFound();
        }

        existingPost.Title = postDto.Title;
        existingPost.Content = postDto.Content;

        context.Entry(existingPost).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PostExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Posts/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await context.Posts.FindAsync(id);
        if (post == null)
        {
            return NotFound();
        }

        context.Posts.Remove(post);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool PostExists(int id)
    {
        return context.Posts.Any(e => e.Id == id);
    }
}

public class PostResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Optionally include comments, but they will be CommentResponseDto
    public ICollection<CommentResponseDto> Comments { get; set; } = new List<CommentResponseDto>();
}

public class PostDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required] public string Content { get; set; } = string.Empty;
}