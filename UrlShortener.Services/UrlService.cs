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
        private readonly IDistributedCache _cache; // call Redis
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private readonly Random _random = new Random();

        // Inject cache via constructor
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

            //Save to cache immediately after creation for super fast first click.
            await _cache.SetStringAsync(shortCode, originalUrl, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) // Allow the cache to live for 24 hours.
            });

            return shortCode;
        }

        public async Task<string> GetOriginalUrlAsync(string shortCode)
        {
            //LOOK IN THE CACHE FIRST
            var cachedUrl = await _cache.GetStringAsync(shortCode);
            if (!string.IsNullOrEmpty(cachedUrl))
            {
                //If found in the cache, return it immediately, bypassing the database!
                return cachedUrl;
            }

            // IF THE CACHE IS MISSING (Cache Miss), THEN SEARCH IN THE DATABASE.
            var mapping = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (mapping != null)
            {
                mapping.AccessCount++;
                await _context.SaveChangesAsync();

                // Once you've retrieved it from the database, save it to the cache so you can retrieve it quickly next time.
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

            //Update the database.
            urlItem.OriginalUrl = newUrl;
            await _context.SaveChangesAsync();

            //Clear the old cache so that it automatically retrieves the new link next time. // Important
            await _cache.RemoveAsync(shortCode);

            return true;
        }

        public async Task<bool> DeleteUrlAsync(string shortCode)
        {
            var urlItem = await _context.UrlMappings.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
            if (urlItem == null) return false;

            //Remove from database
            _context.UrlMappings.Remove(urlItem);
            await _context.SaveChangesAsync();

            //Delete it permanently from the cache so that users can no longer access it. // important
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