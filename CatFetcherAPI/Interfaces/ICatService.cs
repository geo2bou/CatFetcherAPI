using CatFetcherAPI.Models;

namespace CatFetcherAPI.Interfaces
{
    public interface ICatService
    {
        Task FetchAndStoreCats();
        Task<CatEntity?> GetCatById(int id);
        Task<List<CatEntity>> GetPagedCats(int page, int pageSize);
        Task<List<CatEntity>> GetCatsByTag(string tag, int page, int pageSize);
        Task AddCat(CatEntity cat);
    }
}
