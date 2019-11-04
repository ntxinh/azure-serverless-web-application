using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Config
{
    class SampleConfig : IEntityTypeConfiguration<Sample>
    {
        public void Configure(EntityTypeBuilder<Sample> builder)
        {
            builder.Property(bi => bi.Email).IsRequired(true);

            // Make the default table name
            builder.ToTable("Samples");
        }
    }
}
