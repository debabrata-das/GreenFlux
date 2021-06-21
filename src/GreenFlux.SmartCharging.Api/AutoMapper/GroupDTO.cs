using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("GreenFlux.SmartCharging.UnitTests")]
namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class GroupDTO
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public float Capacity { get; set; }
        internal bool FromPost { get; set; }
    }
}