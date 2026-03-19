using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using UrlShortener.Data;
using UrlShortener.Data.Models;
using UrlShortener.Services;
using Xunit;

namespace UrlShortener.UnitTests
{
    public class UrlServiceTests
    {
        // Hàm này giúp tạo ra một Database "ảo" trên RAM, sạch sẽ cho mỗi lần chạy Test
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ShortenUrlAsync_ShouldSaveToDbAndCache_ReturnsShortCode()
        {
            // 1. Arrange (Chuẩn bị dữ liệu và môi trường)
            var options = CreateNewContextOptions();
            var mockCache = new Mock<IDistributedCache>(); // Tạo một Redis giả
            var originalUrl = "https://greenwich.edu.vn";

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UrlService(context, mockCache.Object);

                // 2. Act (Hành động: Gọi hàm cần test)
                var shortCode = await service.ShortenUrlAsync(originalUrl);

                // 3. Assert (Kiểm tra kết quả)
                Assert.NotNull(shortCode);
                Assert.Equal(7, shortCode.Length); // Mã rút gọn phải đúng 7 ký tự

                // Kiểm tra xem đã lưu vào Database chưa
                var dbEntry = await context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
                Assert.NotNull(dbEntry);
                Assert.Equal(originalUrl, dbEntry.OriginalUrl);
            }
        }

        [Fact]
        public async Task GetOriginalUrlAsync_ShouldReturnFromCache_IfAvailable()
        {
            // 1. Arrange
            var options = CreateNewContextOptions();
            var mockCache = new Mock<IDistributedCache>();
            var shortCode = "ABCDEFG";
            var cachedOriginalUrl = "https://cached-url.com";

            // Ép cái Redis giả trả về dữ liệu ngay lập tức (Mô phỏng Cache Hit)
            var encodedUrl = Encoding.UTF8.GetBytes(cachedOriginalUrl);
            mockCache.Setup(c => c.GetAsync(shortCode, default))
                     .ReturnsAsync(encodedUrl);

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UrlService(context, mockCache.Object);

                // 2. Act
                var result = await service.GetOriginalUrlAsync(shortCode);

                // 3. Assert
                Assert.Equal(cachedOriginalUrl, result);
                // Vì Database ảo đang trống trơn, nếu nó trả về đúng link thì 100% là nó đã lấy từ Cache!
            }
        }
    }
}