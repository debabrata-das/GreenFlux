using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenFlux.SmartCharging.Persistence.DbConfiguration
{
    class GroupDbConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");

            builder.HasKey(g => g.Identifier);

            builder.Property(g => g.Name).IsRequired();

            builder.Property(g => g.Capacity).IsRequired();

            builder.HasMany(group => group.ChargeStations)
                .WithOne(c => c.Group)
                .HasForeignKey(chargeStation => chargeStation.GroupIdentifier)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
