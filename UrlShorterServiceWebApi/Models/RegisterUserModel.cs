using System.ComponentModel.DataAnnotations;

namespace UrlShorterServiceWebApi.Models
{
    public class RegisterUserModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords are not similar!")]
        public string ConfirmPassword { get; set; }
    }
}
