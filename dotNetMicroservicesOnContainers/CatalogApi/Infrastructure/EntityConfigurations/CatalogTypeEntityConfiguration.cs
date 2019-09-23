using CatalogApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogApi.Infrastructure.EntityConfigurations
{
    public class CatalogTypeEntityConfiguration : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ForSqlServerUseSequenceHiLo("catalog_type_hilo").IsRequired();

            builder.Property(x => x.Type).IsRequired().HasMaxLength(100);
        }
    }
}