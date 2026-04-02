using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class LikesModel
    {
        [Key]
        public int like_id { get; set; }

        [Required]
        public int community_id { get; set; }

        [Required]
        public int user_id { get; set; }


        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }

        [ForeignKey("community_id")]
        public virtual CommunityModel Community { get; set; }
    }
}
