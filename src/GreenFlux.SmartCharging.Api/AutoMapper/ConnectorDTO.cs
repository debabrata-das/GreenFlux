using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GreenFlux.SmartCharging.UnitTests")]
namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class ConnectorDTO
    {
        public int Identifier { get; set; }
        public Guid ChargeStationIdentifier { get; set; }
        public float MaxCurrentInAmps { get; set; }
        internal bool FromPost { get; set; }
    }
}