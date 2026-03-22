using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Data;
using UrlShortener.Data.Models;

namespace UrlShortener.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDistributedCache _cache; // Biến dùng để gọi Redis
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _random = new Random();

        // Tiêm Cache vào qua Constructor
        public UrlService(ApplicationDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> ShortenUrlAsync(string originalUrl)
        {
            string shortCode;
            while (true)
            {
                shortCode = GenerateRandomCode(7);
                var exists = await _context.UrlMappings.AnyAsync(u => u.ShortCode == shortCode);
                if (!exists) break;
            }

            var urlMapping = new UrlMapping
            {
                OriginalUrl = originalUrl,
                ShortCode = shortCode
            };

            _context.UrlMappings.Add(urlMapping);
            await _context.SaveChangesAsync();

            // LƯU Ý MERIT: Lưu luôn vào Cache ngay khi vừa tạo xong để lần click đầu tiên siêu nhanh
            await _cache.SetStringAsync(shortCode, originalUrl, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Cho phép Cache sống trong 24 giờ
            });

            return shortCode;
        }

        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            // 1. TÌM TRONG CACHE TRƯỚC
            var cachedUrl = await _cache.GetStringAsync(shortCode);
            if (!string.IsNullOrEmpty(cachedUrl))
            {
                // Nếu thấy trong Cache, trả về ngay lập tức, bỏ qua Database!
                return cachedUrl;
            }

            // 2. NẾU CACHE KHÔNG CÓ (Cache Miss), MỚI TÌM TRONG DATABASE
            var mapping = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (mapping != null)
            {
                mapping.AccessCount++;
                await _context.SaveChangesAsync();

                // Lấy được từ DB rồi thì lưu luôn vào Cache để lần sau lấy cho nhanh
                await _cache.SetStringAsync(shortCode, mapping.OriginalUrl, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });

                return mapping.OriginalUrl;
            }

            return null;
        }

        public async Task<bool> UpdateUrlAsync(string shortCode, string newUrl)
        {
            var urlItem = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (urlItem == null) return false;

            // 1. Cập nhật vào Database
            urlItem.OriginalUrl = newUrl;
            await _context.SaveChangesAsync();

            // 2. [TỐI QUAN TRỌNG] Xóa Cache cũ để lần sau nó tự lấy link mới
            await _cache.RemoveAsync(shortCode);

            return true;
        }

        public async Task<bool> DeleteUrlAsync(string shortCode)
        {
            var urlItem = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (urlItem == null) return false;

            // 1. Xóa khỏi Database
            _context.UrlMappings.Remove(urlItem);
            await _context.SaveChangesAsync();

            // 2. [TỐI QUAN TRỌNG] Xóa luôn khỏi Cache để người dùng không truy cập được nữa
            await _cache.RemoveAsync(shortCode);

            return true;
        }

        private string GenerateRandomCode(int length)
        {
            var chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = Alphabet[_random.Next(Alphabet.Length)];
            }
            return new string(chars);
        }
    }
}