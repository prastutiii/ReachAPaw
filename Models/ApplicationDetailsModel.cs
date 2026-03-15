using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public enum HomeType
    {
        Apartment = 0,
        House = 1,
        Condo = 2,
        Other = 3
    }

    public enum OwnOrRent
    {
        Own = 0,
        Rent = 1
    }

    [Table("ApplicationDetails")]
    public class ApplicationDetailsModel
    {
        [Key]
        public int application_id { get; set; }   // PK + FK to AdoptionApplication

        [Required]
        [StringLength(100)]
        public string full_name { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [Phone]
        public string phone { get; set; }

        [Required]
        [StringLength(100)]
        public string city { get; set; }

        [Required]
        public string address { get; set; }

        public HomeType home_type { get; set; }    // int in SQL

        public OwnOrRent own_or_rent { get; set; } // int in SQL

        public bool has_yard { get; set; }

        public bool has_children { get; set; }

        public bool other_pets { get; set; }

        public string pet_experience { get; set; } // Text

        public string reason { get; set; }         // Text

        [StringLength(200)]
        public string valid_document { get; set; }

        // Navigation property back to master
        public virtual AdoptionApplicationModel AdoptionApplications { get; set; }
    }
}