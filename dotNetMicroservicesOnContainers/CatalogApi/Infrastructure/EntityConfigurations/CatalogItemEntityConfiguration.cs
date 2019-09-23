using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogApi.Infrastructure.EntityConfigurations
{
    public class CatalogItemEntityConfiguration : IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("Catalog");

            builder.Property(x => x.Id).ForSqlServerUseSequenceHiLo("catalog_hilo").IsRequired();

            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);

            builder.Property(x => x.Price).IsRequired();

            builder.Property(x => x.PictureFileName).IsRequired(false);

            builder.Property(x => x.PictureUri).IsRequired(false);

            builder.Ignore(x => x.PictureUri);

            builder.HasOne(x => x.CatalogBrand).WithMany().HasForeignKey(x => x.CatalogBrandId);

            builder.HasOne(x => x.CatalogType).WithMany().HasForeignKey(x => x.CatalogTypeId);
        }
    }
}