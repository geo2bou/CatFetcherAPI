using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CatFetcherAPI.Models
{
    public class CatEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CatId { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Width must be greater than 0.")]
        public int Width { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Width must be greater than 0.")]
        public int Height { get; set; }

        [Required]
        [MaxLength(500)]
        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public ICollection<CatTag> CatTags { get; set; } = new List<CatTag>();
    }
}
