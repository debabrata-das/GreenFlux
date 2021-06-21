using System;

namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class ChargeStationDTO
    {
        public Guid Identifier { get; set; }
        public Guid GroupIdentifier { get; set; }
        public string Name { get; set; }
        public bool FromPost { get; set; }
    }
}