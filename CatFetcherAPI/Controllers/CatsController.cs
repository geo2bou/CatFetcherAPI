using CatFetcherAPI.Data;
using CatFetcherAPI.Interfaces;
using CatFetcherAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatFetcherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICatService _catService;

        public CatsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory, ICatService catService)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _catService = catService;
        }

        /// <summary>
        /// Fetches 25 cats from external API and stores them.
        /// </summary>
        [HttpPost("fetch")]
        public async Task<IActionResult> FetchCats()
        {
            await _catService.FetchAndStoreCats();
            return Ok("Cats fetched and stored successfully");
        }

        /// <summary>
        /// Retrieve a cat by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatById(int id)
        {
            var cat = await _catService.GetCatById(id);

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                cat.Id,
                cat.CatId,
                cat.ImageUrl,
                cat.Width,
                cat.Height,
                cat.Created,
                Tags = cat.CatTags.Select(ct => ct.Tag?.Name)
            });
        }

        /// <summary>
        /// Retrieve cats with a specific tag with paging support
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCats([FromQuery] string? tag, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            List<CatEntity> cats;

            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page and pageSize must be greater than 0.");
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                cats = await _catService.GetCatsByTag(tag, page, pageSize);
            }
            else
            {
                cats = await _catService.GetPagedCats(page, pageSize);
            }

            if (cats == null)
            {
                return NotFound();
            }

            var result = cats.Select(cat => new
            {
                cat.Id,
                cat.CatId,
                cat.Width,
                cat.Height,
                cat.ImageUrl,
                Tags = cat.CatTags.Select(ct => ct.Tag?.Name),
                cat.Created
            });

            return Ok(result);
        }
    }
}
