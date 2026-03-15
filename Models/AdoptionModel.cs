using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class AdoptionModel
    {
        [Key]
        public int finalization_id { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public string certificate_number { get; set; }

        [Required]
        public int application_id { get; set; }
        public int payment_id { get; set; }


        [ForeignKey("application_id")]
        public virtual AdoptionApplicationModel AdoptionApplications { get; set; }
        [ForeignKey("payment_id")]
        public virtual PaymentModel Payments { get; set; }
    }
}