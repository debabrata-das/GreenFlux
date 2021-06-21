using AutoMapper;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Api.AutoMapper
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile()
        {
            CreateMap<Group, GroupDTO>().ForMember(x => x.FromPost, opt => opt.Ignore());
            CreateMap<GroupDTO, Group>();
            CreateMap<ChargeStation, ChargeStationDTO>().ForMember(x => x.FromPost, opt => opt.Ignore());
            CreateMap<ChargeStationDTO, ChargeStation>();
            CreateMap<Connector, ConnectorDTO>().ForMember(x => x.FromPost, opt => opt.Ignore());
            CreateMap<ConnectorDTO, Connector>();
        }
    }
}
