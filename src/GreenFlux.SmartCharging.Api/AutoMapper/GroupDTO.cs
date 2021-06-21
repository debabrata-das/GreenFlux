using System;

namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class GroupDTO
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public float Capacity { get; set; }
        public bool FromPost { get; set; }
    }
}