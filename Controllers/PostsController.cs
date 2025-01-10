using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloggingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly BlogDbContext _context;

        private readonly ILogger<PostsController> _logger;

        public PostsController(ILogger<PostsController> logger, BlogDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts([FromQuery] string? terms)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(terms))
            {
                query = query.Where(post =>
                    post.Title.Contains(terms) ||
                    post.Content.Contains(terms) ||
                    post.Category.Contains(terms));
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return post;
        }

        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if(existingPost != null)
            {
                return BadRequest($"Não é possível criar post, Post de Id: {post.Id} já existe");
            }

            if (post == null)
            {
                return NotFound();
            }
            post.CreatedAt = DateTime.Now;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.Category = post.Category;
            existingPost.Tags = post.Tags;
            existingPost.UpdatedAt = DateTime.Now;

            _context.Entry(existingPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}
