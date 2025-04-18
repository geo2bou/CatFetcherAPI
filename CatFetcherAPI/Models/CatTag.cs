namespace CatFetcherAPI.Models
{
    public class CatTag
    {
        public int CatEntityId { get; set; }
        public CatEntity Cat { get; set; } = null!;

        public int TagEntityId { get; set; }
        public TagEntity Tag { get; set; } = null!;
    }
}
