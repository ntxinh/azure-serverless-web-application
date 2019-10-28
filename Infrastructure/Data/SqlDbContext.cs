using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data
{
    public class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
        {
        }

        public DbSet<Sample> Samples { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Sample>(ConfigureSample);
        }

        private void ConfigureSample(EntityTypeBuilder<Sample> builder)
        {
            builder.ToTable("Samples");
            builder.Property(bi => bi.Email)
                .IsRequired(true);
        }
    }
}
