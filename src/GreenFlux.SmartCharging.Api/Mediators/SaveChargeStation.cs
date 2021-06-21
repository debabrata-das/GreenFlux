using GreenFlux.SmartCharging.Api.AutoMapper;
using MediatR;

namespace GreenFlux.SmartCharging.Api.Mediators
{
    public class SaveChargeStation : ChargeStationDTO, IRequest<SaveChargeStationOutput> { }
}