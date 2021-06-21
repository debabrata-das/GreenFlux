using System;
using System.Collections.Generic;

namespace GreenFlux.SmartCharging.Domain.Models
{
    public class Group
    {
        public Guid Identifier { get; set; }

        public string Name { get; set; }

        public float Capacity { get; set; }

        public HashSet<ChargeStation> ChargeStations { get; set; }

        public Group()
        {
            ChargeStations = new HashSet<ChargeStation>(new ChargeStationComparer());
        }
    }
}
