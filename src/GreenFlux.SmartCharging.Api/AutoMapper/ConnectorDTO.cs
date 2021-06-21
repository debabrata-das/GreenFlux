using System;

namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class ConnectorDTO
    {
        public int Identifier { get; set; }
        public Guid ChargeStationIdentifier { get; set; }
        public float MaxCurrentInAmps { get; set; }
        public bool FromPost { get; set; }
    }
}