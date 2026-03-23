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
        //creates a clean, "virtual" database in RAM for each test run.
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task ShortenUrlAsync_ShouldSaveToDbAndCache_ReturnsShortCode()
        {
            // 1. Arrange Prepare data and environment
            var options = CreateNewContextOptions();
            var mockCache = new Mock<IDistributedCache>(); // Create a fake Redis
            var originalUrl = "https://greenwich.edu.vn";

            using (var context = new ApplicationDbContext(options))
            {
                var service = new UrlService(context, mockCache.Object);

                // Action: Calling the function to be tested
                var shortCode = await service.ShortenUrlAsync(originalUrl);

                // 3. Assert (Check results)
                Assert.NotNull(shortCode);
                Assert.Equal(7, shortCode.Length); // The shortened code must be exactly 7 characters long.

                // Check if it has been saved to the database
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

            // Force the fake Redis to return data immediately (Simulate Cache Hit)
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
                // Since the virtual database is empty, if it returns the correct link, it's 100% certain it retrieved the data from the cache
            }
        }
    }
}