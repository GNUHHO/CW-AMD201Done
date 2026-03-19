using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UrlShortener.Services;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("")] // Để route trống để URL ngắn đẹp hơn (vd: localhost:5000/3StkRt6)
    public class UrlController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlController(IUrlService urlService)
        {
            _urlService = urlService;
        }

        // Tạo một Class nhỏ để nhận Data từ người dùng (Bạn có thể chuyển file này sang project Common sau)
        public class UrlRequestDto
        {
            public string Url { get; set; }
        }

        // API 1: Tạo link rút gọn [POST /api/shorten]
        [HttpPost("api/shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] UrlRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Url))
            {
                return BadRequest("URL cannot be empty.");
            }

            var shortCode = await _urlService.ShortenUrlAsync(request.Url);

            // Lấy ra domain hiện tại để nối thành link hoàn chỉnh
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";

            return Ok(new { ShortUrl = shortUrl });
        }

        // API 2: Chuyển hướng khi truy cập link rút gọn [GET /{code}]
        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var originalUrl = await _urlService.GetOriginalUrlAsync(code);

            if (originalUrl == null)
            {
                return NotFound("Short link not found.");
            }

            // Trả về mã lỗi 302 Found để trình duyệt tự động chuyển hướng
            return Redirect(originalUrl);
        }
    }
}