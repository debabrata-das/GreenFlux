using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GreenFlux.SmartCharging.Persistence.DbConfiguration
{
    class ConnectorDbConfiguration : IEntityTypeConfiguration<Connector>
    {
        public void Configure(EntityTypeBuilder<Connector> builder)
        {
            builder.ToTable("Connectors");
            
            builder.HasKey(connector => new {connector.Identifier, connector.ChargeStationIdentifier});
            
            builder.Property(connector => connector.Identifier).ValueGeneratedNever();
            
            builder.Property(connector => connector.ChargeStationIdentifier).IsRequired();
         
            builder.Property(connector => connector.MaxCurrentInAmps).IsRequired();
        }
    }
}
