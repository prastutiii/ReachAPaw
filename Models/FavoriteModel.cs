using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class FavoriteModel
    {
        [Key]
        public int favorite_id { get; set; }

        [Required]
        public int user_id { get; set; }

        [Required]
        public int pet_id { get; set; }

        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }

        [ForeignKey("pet_id")]
        public virtual PetModel Pets { get; set; }
    }
}