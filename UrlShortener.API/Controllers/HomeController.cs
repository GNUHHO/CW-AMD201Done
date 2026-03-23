using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UrlShortener.Services;
using UrlShortener.Data;
using System.Linq;

namespace UrlShortener.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUrlService _urlService;
        private readonly ApplicationDbContext _context;

        public HomeController(IUrlService urlService, ApplicationDbContext context)
        {
            _urlService = urlService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var history = await _context.UrlMappings
                                        .OrderByDescending(u => u.CreatedAt)
                                        .Take(10)
                                        .ToListAsync();

            ViewBag.History = history;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Shorten(string originalUrl)
        {
            if (string.IsNullOrWhiteSpace(originalUrl))
            {
                TempData["Error"] = "Please enter a valid URL!";
                return RedirectToAction("Index");
            }

            var shortCode = await _urlService.ShortenUrlAsync(originalUrl);
            var shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";

            TempData["ShortUrl"] = shortUrl;
            TempData["OriginalUrl"] = originalUrl;

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAll()
        {
            var allLinks = await _context.UrlMappings.ToListAsync();

            if (allLinks.Any())
            {
                _context.UrlMappings.RemoveRange(allLinks);
                await _context.SaveChangesAsync();

                TempData["SuccessMsg"] = "All URL history has been cleared successfully!";
            }

            return RedirectToAction("Index");
        }

        // MOST IMPORTANT FUNCTION: REFERRING SHORTENED LINKS
        [HttpGet("/{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            if (string.IsNullOrEmpty(shortCode)) return RedirectToAction("Index");

            var link = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (link == null)
            {
                TempData["Error"] = "Oops! The short link does not exist or has been deleted.";
                return RedirectToAction("Index");
            }

            // Increase the number of clicks by 1
            link.AccessCount += 1;
            await _context.SaveChangesAsync();

            // Redirect
            return Redirect(link.OriginalUrl);
        }
    }
}