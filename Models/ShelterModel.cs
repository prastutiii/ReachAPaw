using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class ShelterModel
    {
        [Key]
        public int shelter_id { get; set; }

        [Required]
        public string shelter_name { get; set; }

        [EmailAddress]
        public string? email { get; set; }

        public string? phone { get; set; }

        public string? address { get; set; }

        public string? city { get; set; }

        public string? pan_number { get; set; }

        public string? website { get; set; }

        public string? hours { get; set; }
        public string? description { get; set; }

        public string? status { get; set; }
        public string? shelter_img { get; set; }
        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public UserModel? Users { get; set; }
        public ICollection<PetModel> Pets { get; set; }
    }
}
