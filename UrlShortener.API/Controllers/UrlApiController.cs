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

        // Tiêm cả Service (để tạo link) và DbContext (để lấy/sửa/xóa)
        public UrlApiController(IUrlService urlService, ApplicationDbContext context)
        {
            _urlService = urlService;
            _context = context;
        }

        // ==========================================
        // 1. C - CREATE: Tạo link rút gọn mới
        // ==========================================
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

        // ==========================================
        // 2. R - READ: Lấy danh sách TOÀN BỘ link
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var links = await _context.UrlMappings
                                      .OrderByDescending(x => x.CreatedAt)
                                      .ToListAsync();
            return Ok(links);
        }

        // ==========================================
        // 3. R - READ: Lấy thông tin 1 link theo ShortCode
        // ==========================================
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetByCode(string shortCode)
        {
            var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (link == null) return NotFound(new { message = "Short code not found!" });

            return Ok(link);
        }

        // ==========================================
        // 4. U - UPDATE: Sửa đường link gốc
        // ==========================================
        [HttpPut("{shortCode}")]
        public async Task<IActionResult> Update(string shortCode, [FromBody] string newOriginalUrl)
        {
            if (string.IsNullOrWhiteSpace(newOriginalUrl)) return BadRequest(new { message = "Invalid new URL" });

            // Tìm link trong Database
            var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
            if (link == null) return NotFound(new { message = "Short code not found!" });

            // Cập nhật link mới
            link.OriginalUrl = newOriginalUrl;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Updated successfully", Data = link });
        }

        // ==========================================
        // 5. D - DELETE: Xóa 1 link cụ thể
        // ==========================================
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