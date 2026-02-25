using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class PetModel
    {
        [Key]
        public int pet_id { get; set; }

        [Required]
        public string pet_name { get; set; }

        public string species { get; set; }

        public string gender { get; set; }

        public string age { get; set; }

        public string location { get; set; }

        public string fee { get; set; }

        public string description { get; set; }

        public string ideal_home { get; set; }

        public string health_status { get; set; }

        public bool is_vaccinated { get; set; }

        public bool is_neutered { get; set; }

        public bool is_microchipped { get; set; }

        public string status { get; set; }
        public string? image_url { get; set; }

        //Foreign Key
        public int shelter_id { get; set; }

        [ForeignKey("shelter_id")]
        public ShelterModel? Shelters { get; set; }
    }
}
