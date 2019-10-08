using CatalogApi.Infrastructure.EntityConfigurations;
using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogApi.Infrastructure
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public DbSet<CatalogItem> CatalogItems { get; set; }
        public DbSet<CatalogBrand> CatalogBrands { get; set; }
        public DbSet<CatalogType> CatalogTypes { get; set; }

        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new CatalogItemEntityConfiguration());
            builder.ApplyConfiguration(new CatalogBrandEntityConfiguration());
            builder.ApplyConfiguration(new CatalogTypeEntityConfiguration());
        }*/
    }
}