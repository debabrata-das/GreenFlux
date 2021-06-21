using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("GreenFlux.SmartCharging.UnitTests")]
namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class ChargeStationDTO
    {
        public Guid Identifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public string Name { get; set; }
        internal bool FromPost { get; set; }
    }
}