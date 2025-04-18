using System;
using CatFetcherAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatFetcherAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }

        public DbSet<CatEntity> Cats => Set<CatEntity>();
        public DbSet<TagEntity> Tags => Set<TagEntity>();
        public DbSet<CatTag> CatTags => Set<CatTag>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatTag>().HasKey(ct => new { ct.CatEntityId, ct.TagEntityId });
            modelBuilder.Entity<CatTag>().HasOne(ct => ct.Cat).WithMany(c => c.CatTags).HasForeignKey(ct => ct.CatEntityId);
            modelBuilder.Entity<CatTag>().HasOne(ct => ct.Tag).WithMany(t => t.CatTags).HasForeignKey(ct => ct.TagEntityId);
        }
    }
}
