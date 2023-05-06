using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UrlModel
    {
        [Required]
        [Url]
        public string OriginalUrl { get; set; }
        [Required]
        public string ShortUrl { get; set; }
    }
}
