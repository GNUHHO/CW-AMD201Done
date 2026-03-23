using System;
using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Data.Models
{
    public class UrlMapping
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AccessCount { get; set; } = 0;
    }
}