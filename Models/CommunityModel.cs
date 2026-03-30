using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class CommunityModel
    {
        [Key]
        public int community_id { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public string post_content { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public int category_id { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;

        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }

        [ForeignKey("category_id")]
        public virtual CategoryModel Categories { get; set; }
    }
}