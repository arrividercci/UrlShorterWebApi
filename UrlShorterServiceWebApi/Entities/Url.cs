using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Entities
{
    public class Url
    {
        public int Id { get; set; }
        [Required]
        public string OriginalUrl { get; set; }
        [Required]
        public string HashCodeUrl { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
    }
}
