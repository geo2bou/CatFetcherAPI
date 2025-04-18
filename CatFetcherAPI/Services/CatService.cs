using System.Net.Http;
using System.Text.Json;
using Azure;
using CatFetcherAPI.Data;
using CatFetcherAPI.Interfaces;
using CatFetcherAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatFetcherAPI.Services
{
    public class CatService : ICatService
    {
        public readonly HttpClient _httpClient;
        public readonly ApplicationDbContext _dbContext;

        public CatService(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task FetchAndStoreCats()
        {
            var requestUrl = "https://api.thecatapi.com/v1/images/search?limit=25&has_breeds=1";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch cats from API");
            }

            var content = await response.Content.ReadAsStringAsync();
            var catDataList = JsonSerializer.Deserialize<List<CatApiResponse>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (catDataList == null || !catDataList.Any())
            {
                return;
            }

            foreach (var catData in catDataList)
            {
                if (await _dbContext.Cats.AnyAsync(c => c.CatId == catData.Id))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(catData.Url) ||
                    string.IsNullOrWhiteSpace(catData.Id) ||
                    catData.Width <= 0 ||
                    catData.Height <= 0)
                {
                    continue;
                }

                var breed = catData.Breeds?.FirstOrDefault();
                var temperament = breed?.Temperament;

                var tagNames = !string.IsNullOrWhiteSpace(temperament)
                    ? temperament.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToList()
                    : new List<string>();

                var catEntity = new CatEntity
                {
                    CatId = catData.Id,
                    Width = catData.Width,
                    Height = catData.Height,
                    ImageUrl = catData.Url,
                    Created = DateTime.UtcNow,
                    CatTags = new List<CatTag>()
                };

                foreach (var tagName in tagNames)
                {
                    var existingTag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
                    if (existingTag == null)
                    {
                        existingTag = new TagEntity
                        {
                            Name = tagName,
                            Created = DateTime.UtcNow
                        };
                        _dbContext.Tags.Add(existingTag);
                        await _dbContext.SaveChangesAsync();
                    }

                    catEntity.CatTags.Add(new CatTag
                    {
                        TagEntityId = existingTag.Id,
                        Tag = existingTag
                    });
                }

                _dbContext.Cats.Add(catEntity);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<CatEntity?> GetCatById(int id)
        {
            return await _dbContext.Cats.Include(c => c.CatTags).ThenInclude(ct => ct.Tag).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CatEntity>> GetPagedCats(int page, int pageSize)
        {
            return await _dbContext.Cats
                .Include(c => c.CatTags)
                .ThenInclude(ct => ct.Tag)
                .OrderByDescending(c => c.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<CatEntity>> GetCatsByTag(string tag, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
            }

            return await _dbContext.Cats
                .Include(c => c.CatTags)
                .ThenInclude(ct => ct.Tag)
                .Where(c => c.CatTags.Any(ct => ct.Tag != null && ct.Tag.Name.ToLower() == tag.ToLower()))
                .OrderByDescending(c => c.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task AddCat(CatEntity cat)
        {
            _dbContext.Cats.Add(cat);
            await _dbContext.SaveChangesAsync();
        }
    }
}
