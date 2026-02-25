using System.ComponentModel.DataAnnotations;

namespace ReachAPaw.Models
{
    public class ShelterModel
    {
        [Key]
        public int shelter_id { get; set; }

        [Required]
        public string shelter_name { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public string phone { get; set; }

        public string address { get; set; }

        public string city { get; set; }

        public string pan_number { get; set; }

        public string website { get; set; }

        public string hours { get; set; }

        public string status { get; set; }

        public ICollection<PetModel> Pets { get; set; }
    }
}
