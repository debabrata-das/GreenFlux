using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public interface IChargeStationRepository
    {
        Task Add(ChargeStation chargeStation);
        Task<ChargeStation> GetByIdentifier(Guid identifier);
        Task RemoveChargeStation(Guid identifier);
        Task RemoveChargeStation(ChargeStation chargeStation);
        Task<IList<ChargeStation>> GetByGroupIdentifier(Guid groupIdentifier);
        Task<float> GetMaxCurrentInAmps(ChargeStation chargeStation);
    }
}