using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UrlDto
    {
        [Required]
        [Url]
        public string Url { get; set; }
    }
}
