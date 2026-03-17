using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class NotificationModel
    {
        [Key]
        public int notification_id { get; set; }

        [Required]
        public string message { get; set; }

        public bool is_read { get; set; } = false;

        [Required]
        public DateTime created_at { get; set; }

        [Required]
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }
    }
}