using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UrlDto
    {
        [Required]
        public string Url { get; set; }
    }
}
