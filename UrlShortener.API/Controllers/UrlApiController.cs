using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UrlShortener.Services;
using UrlShortener.Data;

namespace UrlShortener.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlApiController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly ApplicationDbContext _context;

        // Inject both Service to create links
        // DbContext (to retrieve/edit/delete)
        public UrlApiController(IUrlService urlService, ApplicationDbContext context)
        {
            _urlService = urlService;
            _context = context;
        }

        // 1.CREATE
        [HttpPost("shorten")]
        public async Task<IActionResult> Create([FromBody] string originalUrl)
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                return BadRequest(new { message = "Invalid URL" });
            }

            var shortCode = await _urlService.ShortenUrlAsync(originalUrl);
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";

            return Ok(new
            {
                Message = "Created successfully",
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl,
                ShortCode = shortCode
            });
        }

        // READ
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var links = await _context.UrlMappings
                                      .OrderByDescending(x => x.CreatedAt)
                                      .ToListAsync();
            return Ok(links);
        }

        // READ: Get information from a link using a Shortcode

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetByCode(string shortCode)
        {
            var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (link == null) return NotFound(new { message = "Short code not found!" });

            return Ok(link);
        }

        // UPDATE
        [HttpPut("{shortCode}")]
        public async Task<IActionResult> Update(string shortCode, [FromBody] string newOriginalUrl)
        {
            if (string.IsNullOrWhiteSpace(newOriginalUrl)) return BadRequest(new { message = "Invalid new URL" });

            // Find the link in the database.
            var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
            if (link == null) return NotFound(new { message = "Short code not found!" });

            // Update the new link.
            link.OriginalUrl = newOriginalUrl;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Updated successfully", Data = link });
        }

        //DELETE
        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> Delete(string shortCode)
        {
            var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
            if (link == null) return NotFound(new { message = "Short code not found!" });

            _context.UrlMappings.Remove(link);
            await _context.SaveChangesAsync();

            return Ok(new { Message = $"Deleted short code '{shortCode}' successfully!" });
        }
    }
}