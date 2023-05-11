using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UrlToGet
    {
        public int Id { get; set; }
        [Url]
        public string? OriginalUrl { get; set; }
        public string? ShortUrl { get; set; }

        public string? CodeUrl { get; set; }
    }
}
