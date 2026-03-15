using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    [Table("AdoptionApplication")]
    public class AdoptionApplicationModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int adoption_id { get; set; }   // PK

        [Required]
        [StringLength(50)]
        public string status { get; set; }        // e.g., Pending, Approved, Rejected

        [Required]
        public DateTime applied_date { get; set; }

        // Foreign Keys
        [Required]
        public int user_id { get; set; }

        [Required]
        public int shelter_id { get; set; }

        [Required]
        public int pet_id { get; set; }

        [Required]
        public int application_id { get; set; }

        // Navigation property to details
        [ForeignKey("pet_id")]
        public virtual PetModel Pets { get; set; }
        [ForeignKey("application_id")]
        public virtual ApplicationDetailsModel ApplicationDetails { get; set; }
        [ForeignKey("user_id")]
        public virtual UserModel Users { get; set; }
        [ForeignKey("shelter_id")]
        public virtual ShelterModel Shelters { get; set; }
    }
}
