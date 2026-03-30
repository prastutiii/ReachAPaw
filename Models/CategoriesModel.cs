using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReachAPaw.Models
{
    public class CategoryModel
    {
        [Key]
        public int category_id { get; set; }

        [Required]
        public string category_name { get; set; }

        public ICollection<CommunityModel> Community { get; set; }
    }
}