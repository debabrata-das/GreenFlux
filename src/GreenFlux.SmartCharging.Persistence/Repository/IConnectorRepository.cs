using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public interface IConnectorRepository
    {
        Task AddConnector(Connector connector);
        Task<Connector> GetByIdentifierAndChargeStation(int identifier, Guid chargeStationIdentifier);
        Task RemoveConnectorByIdentifierAndChargeStation(int identifier, Guid chargeStationIdentifier);
        Task RemoveConnector(Connector connector);
        Task<IList<Connector>> GetByChargeStationIdentifier(Guid identifier);
    }
}