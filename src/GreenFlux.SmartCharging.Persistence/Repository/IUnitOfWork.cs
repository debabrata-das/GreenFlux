using System;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public interface IUnitOfWork
    {
        IConnectorRepository ConnectorRepository { get; }
        IChargeStationRepository ChargeStationRepository { get; }
        IGroupRepository GroupRepository { get; }
        Task<int> SaveAsync();
        Task RemoveChargeStation(Guid identifier);
        Task RemoveGroup(Guid identifier);
        Task<(bool, float)> CheckIfCanAddToCurrentCapacityAndGetCurrentTotalMaxAmpsForGroup(Group group, float capacityToBeAdded);
        Task CheckGroupCapacity(Group existingGroup, float newCapacity);
    }
}
