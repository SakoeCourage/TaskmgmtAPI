using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TaskmgmtAPI.Models
{
    [Table("task")]
    public class UserTask
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int authorID { get; set; }
        public virtual User author { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public string description { get; set; }
        public Boolean isCompleted { get; set; } = false;
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Timestamp]
        public DateTime createdAt { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Timestamp]
        public DateTime updatedAt { get; set; }


    }
}
