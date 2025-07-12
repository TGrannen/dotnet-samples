namespace TUnitTesting.WebApi.Controllers;

[Route("api/posts/{postId}/comments")]
[ApiController]
public class CommentsController(ApplicationDbContext context) : ControllerBase
{
    // GET: api/posts/5/comments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetCommentsForPost(int postId)
    {
        var postExists = await context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            return NotFound($"Post with ID {postId} not found.");
        }

        var comments = await context.Comments
            .Where(c => c.PostId == postId)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                PostId = c.PostId
            })
            .ToListAsync();
        return Ok(comments);
    }

    // GET: api/posts/5/comments/10
    [HttpGet("{id}")]
    public async Task<ActionResult<CommentResponseDto>> GetComment(int postId, int id)
    {
        var comment = await context.Comments
            .Where(c => c.Id == id && c.PostId == postId)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Text = c.Text,
                CreatedAt = c.CreatedAt,
                PostId = c.PostId
            })
            .FirstOrDefaultAsync();

        if (comment == null)
        {
            return NotFound($"Comment with ID {id} not found for Post ID {postId}.");
        }

        return Ok(comment);
    }

    // POST: api/posts/5/comments
    // This endpoint still takes a CommentDto for input
    [HttpPost]
    public async Task<ActionResult<CommentResponseDto>> CreateComment(int postId, CommentDto commentDto)
    {
        var postExists = await context.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            return NotFound($"Post with ID {postId} not found.");
        }

        var comment = new Comment
        {
            Text = commentDto.Text,
            PostId = postId,
            CreatedAt = DateTime.UtcNow
        };

        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        // Return the newly created comment as a DTO
        var responseDto = new CommentResponseDto
        {
            Id = comment.Id,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            PostId = comment.PostId
        };

        return CreatedAtAction(nameof(GetComment), new { postId = postId, id = comment.Id }, responseDto);
    }

    // PUT: api/posts/5/comments/10
    // This endpoint still takes a CommentDto for input
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int postId, int id, CommentDto commentDto)
    {
        var existingComment = await context.Comments.FindAsync(id);

        if (existingComment == null || existingComment.PostId != postId)
        {
            return NotFound($"Comment with ID {id} not found for Post ID {postId}.");
        }

        existingComment.Text = commentDto.Text;

        context.Entry(existingComment).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CommentExists(id, postId))
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

    // DELETE: api/posts/5/comments/10
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(int postId, int id)
    {
        var comment = await context.Comments.FindAsync(id);

        if (comment == null || comment.PostId != postId)
        {
            return NotFound($"Comment with ID {id} not found for Post ID {postId}.");
        }

        context.Comments.Remove(comment);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool CommentExists(int id, int postId)
    {
        return context.Comments.Any(e => e.Id == id && e.PostId == postId);
    }
}

public class CommentDto
{
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Text { get; set; } = string.Empty;
}

public class CommentResponseDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int PostId { get; set; } // We might still want to know the parent PostId
}