using System.ComponentModel.DataAnnotations;

namespace TaskmgmtAPI.Controllers.Auth
{
    public class UserloginDto
    {
        [Required, EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

    }
}
