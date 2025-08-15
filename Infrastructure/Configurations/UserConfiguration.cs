using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.RefreshToken)
                .HasMaxLength(150);

            builder.HasIndex(x => x.RefreshToken)
                .IsUnique();
        }
    }
}
