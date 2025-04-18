using System.ComponentModel.DataAnnotations;

namespace CatFetcherAPI.Models
{
    public class Breed
    {
        [MaxLength(100)]
        public string Temperament { get; set; } = string.Empty;
    }
}
