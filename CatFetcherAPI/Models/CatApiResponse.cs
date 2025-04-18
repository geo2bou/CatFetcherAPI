using System.ComponentModel.DataAnnotations;

namespace CatFetcherAPI.Models
{
    public class CatApiResponse
    {
        public string Id { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public List<Breed> Breeds { get; set; } = new();
    }
}
