using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace TaskmgmtAPI.Controllers.Auth
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        [Compare(nameof(password), ErrorMessage = "Password Confirmation doees not match")]
        public string passwordConfirmation { get; set; }
    }
}
