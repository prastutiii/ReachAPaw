using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class PaymentModel
    {
        [Key]
        public int payment_id { get; set; }

        [Required]
        public double amount { get; set; }

        [Required]
        public string? method { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public int adoption_id { get; set; }

        [ForeignKey("adoption_id")]
        public virtual AdoptionApplicationModel AdoptionApplications { get; set; }
    }
}