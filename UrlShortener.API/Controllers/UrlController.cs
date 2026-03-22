using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UrlShortener.Services;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        public class UrlRequestDto
        {
            public string Url { get; set; }
        }

        public class UpdateUrlRequestDto
        {
            public string NewUrl { get; set; }
        }

        // 1. CREATE [POST]
        [HttpPost("api/shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] UrlRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Url)) return BadRequest("URL cannot be empty.");
            var shortCode = await _urlService.ShortenUrlAsync(request.Url);
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
            return Ok(new { ShortUrl = shortUrl });
        }

        // 2. READ [GET]
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(code);
            if (originalUrl == null) return NotFound("Short link not found.");
            return Redirect(originalUrl);
        }

        // 3. UPDATE [PUT]
        [HttpPut("api/shorten/{code}")]
        public async Task<IActionResult> UpdateUrl(string code, [FromBody] UpdateUrlRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.NewUrl)) return BadRequest("New URL cannot be empty.");

            // Nhờ Service làm việc với Database
            var isUpdated = await _urlService.UpdateUrlAsync(code, request.NewUrl);
            if (!isUpdated) return NotFound("Short link not found.");

            return Ok(new { Message = "Cập nhật thành công!", ShortCode = code, NewUrl = request.NewUrl });
        }

        // 4. DELETE [DELETE]
        [HttpDelete("api/shorten/{code}")]
        public async Task<IActionResult> DeleteUrl(string code)
        {
            // Nhờ Service làm việc với Database
            var isDeleted = await _urlService.DeleteUrlAsync(code);
            if (!isDeleted) return NotFound("Short link not found.");

            return Ok(new { Message = "Xóa thành công!", ShortCode = code });
        }
    }
}