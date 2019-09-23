using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogApi.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityConfiguration : IEntityTypeConfiguration<CatalogBrand>
    {
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand");

            builder.HasKey(x => x.Brand);

            builder.Property(x => x.Id).ForSqlServerUseSequenceHiLo("catalog_brand_hilo").IsRequired();

            builder.Property(x => x.Brand).IsRequired().HasMaxLength(100);
        }
    }
}