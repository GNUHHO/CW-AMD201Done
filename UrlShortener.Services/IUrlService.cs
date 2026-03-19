using System.Threading.Tasks;

namespace UrlShortener.Services
{
    public interface IUrlService
    {
        // Nhận vào URL gốc và trả về mã ngắn (Short Code)
        Task<string> ShortenUrlAsync(string originalUrl);

        // Nhận vào mã ngắn và trả về URL gốc để chuyển hướng
        Task<string> GetOriginalUrlAsync(string shortCode);
    }
}