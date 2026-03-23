using System.Threading.Tasks;

namespace UrlShortener.Services
{
    public interface IUrlService
    {
        // Receives the original URL and returns a shortcode
        Task<string> ShortenUrlAsync(string originalUrl);

        // Receive the shortcode and return the original URL for redirection
        Task<string> GetOriginalUrlAsync(string shortCode);
        Task<bool> UpdateUrlAsync(string shortCode, string newUrl);
        Task<bool> DeleteUrlAsync(string shortCode);
    }
}