using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class CommentsModel
    {
        [Key]
        public int comment_id { get; set; }

        [Required]
        public string comment { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public int user_id { get; set; }
        [Required]
        public int community_id { get; set; }


        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }

        [ForeignKey("community_id")]
        public virtual CommunityModel Community { get; set; }
    }
}
