using System.ComponentModel.DataAnnotations;

namespace ReachAPaw.Models
{
    public class UserModel
    {
        [Key]
        public int user_id { get; set; }

        [Required]
        public string username { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string status { get; set; }
        public string? image_url { get; set; }

        public ICollection<ShelterModel> Shelters { get; set; }
    }
}
