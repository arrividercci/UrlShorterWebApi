using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UrlModel
    {
        [Required]
        public string OriginalUrl { get; set; }
        [Required]
        public string CustomUrl { get; set; }
    }
}
