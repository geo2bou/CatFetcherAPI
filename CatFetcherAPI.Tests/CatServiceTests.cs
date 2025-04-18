using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CatFetcherAPI.Data;
using CatFetcherAPI.Models;
using CatFetcherAPI.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq.Protected;
using Moq;
using System.Text.Json;

namespace CatFetcherAPI.Tests
{
    public class CatServiceTests
    {
        private async Task<ApplicationDbContext> GetInMemoryDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        [Fact]
        public async Task AddCatAsync_ShouldAddCatToDatabase()
        {
            var dbContext = await GetInMemoryDbContextAsync();
            var service = new CatService(null!, dbContext);

            var cat = new CatEntity
            {
                CatId = "abc123",
                Width = 500,
                Height = 300,
                ImageUrl = "https://example.com/cat.jpg"
            };

            await service.AddCat(cat);

            var cats = dbContext.Cats.ToList();
            cats.Should().ContainSingle(c => c.CatId == "abc123");
        }

        private async Task<ApplicationDbContext> GetDbContextAsync()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        private HttpClient GetMockHttpClient(string jsonResponse)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.thecatapi.com/")
            };
        }

        [Fact]
        public async Task FetchAndStoreCatsAsync_ShouldStoreCatsInDatabase()
        {
            var dbContext = await GetDbContextAsync();

            var mockCats = new[]
            {
                new {
                    id = "abc123",
                    width = 640,
                    height = 480,
                    url = "https://example.com/cat1.jpg",
                    breeds = new[] {
                        new { temperament = "Playful, Active" }
                    }
                },
                new {
                    id = "def456",
                    width = 800,
                    height = 600,
                    url = "https://example.com/cat2.jpg",
                    breeds = new[] {
                        new { temperament = "Lazy, Calm" }
                    }
                }
            };

            var jsonResponse = JsonSerializer.Serialize(mockCats);

            var httpClient = GetMockHttpClient(jsonResponse);

            var service = new CatService(httpClient, dbContext);

            await service.FetchAndStoreCats();

            var cats = dbContext.Cats.Include(c => c.CatTags).ThenInclude(ct => ct.Tag).ToList();

            cats.Should().HaveCount(2);
            cats.Should().Contain(c => c.CatId == "abc123" && c.ImageUrl == "https://example.com/cat1.jpg");
            cats.Should().Contain(c => c.CatId == "def456" && c.ImageUrl == "https://example.com/cat2.jpg");

            dbContext.Tags.Should().HaveCount(4);
        }
    }
}
