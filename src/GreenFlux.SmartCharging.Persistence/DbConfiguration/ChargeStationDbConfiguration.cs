using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenFlux.SmartCharging.Persistence.DbConfiguration
{
    class ChargeStationDbConfiguration : IEntityTypeConfiguration<ChargeStation>
    {
        public void Configure(EntityTypeBuilder<ChargeStation> builder)
        {
            builder.ToTable("ChargeStations");

            builder.HasKey(chargeStation => chargeStation.Identifier);

            builder.Property(chargeStation => chargeStation.Name).IsRequired();

            builder.HasMany(chargeStation => chargeStation.Connectors)
                .WithOne(connector => connector.ChargeStation)
                .HasForeignKey(connector => connector.ChargeStationIdentifier)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
