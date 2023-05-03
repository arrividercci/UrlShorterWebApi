using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class UserDto
    {
        [Required]
        public string Token { get; set; }
    }
}
