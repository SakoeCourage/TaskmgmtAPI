using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskmgmtAPI.Models
{
    [Table("user")]
    [Index(nameof(User.email), IsUnique = true)]
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string name { get; set; }

        
        [Required, EmailAddress]
        public string email { get; set; }

        
        [Required, PasswordPropertyText]
        public string password { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Timestamp]
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Timestamp]
        public DateTime updatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<UserTask> UserTasks { get; } = new List<UserTask>();
    }

}
