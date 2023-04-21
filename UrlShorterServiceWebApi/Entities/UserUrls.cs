using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Entities
{
    public class UserUrls
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int UrlId { get; set; }
        public virtual Url Url { get; set; } = null!;
    }
}
