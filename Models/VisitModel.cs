using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class VisitModel
    {
        [Key]
        public int visit_id { get; set; }

        [Required]
        public string? name { get; set; }

        [Required]
        public string? phone { get; set; }

        [Required]
        public DateTime? date { get; set; }

        [Required]
        public TimeSpan? time { get; set; }

        public string? reason { get; set; }

        [Required]
        public string? people_no { get; set; }

        [Required]
        public string? status { get; set; } = "Scheduled";

        [Required]
        public int shelter_id { get; set; }

        [ForeignKey("shelter_id")]
        public virtual ShelterModel Shelters { get; set; }
    }
}