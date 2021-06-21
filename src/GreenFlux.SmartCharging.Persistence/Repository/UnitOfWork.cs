using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GreenFluxDbContext _context;

        public UnitOfWork(GreenFluxDbContext context)
        {
            _context = context;
        }

        private ChargeStationRepository _chargeStationRepository;
        private GroupRepository _groupRepository;
        private ConnectorRepository _connectorRepository;

        public IChargeStationRepository ChargeStationRepository => _chargeStationRepository = _chargeStationRepository ?? new ChargeStationRepository(_context);
        public IGroupRepository GroupRepository => _groupRepository = _groupRepository ?? new GroupRepository(_context);
        public IConnectorRepository ConnectorRepository => _connectorRepository = _connectorRepository ?? new ConnectorRepository(_context);

        public async Task RemoveChargeStation(Guid identifier)
        {
            var chargeStation = await ChargeStationRepository.GetByIdentifier(identifier);
            if (chargeStation != null)
            {
                var connectors = await ConnectorRepository.GetByChargeStationIdentifier(identifier);
                if (connectors.Any())
                {
                    foreach (var connector in connectors)
                    {
                        await ConnectorRepository.RemoveConnector(connector);
                    }
                }
                await ChargeStationRepository.RemoveChargeStation(chargeStation.Identifier);
                await SaveAsync();
            }
        }

        public async Task RemoveGroup(Guid identifier)
        {
            var group = GroupRepository.GetByIdentifier(identifier);
            if (group != null)
            {
                IList<ChargeStation> chargeStations = await ChargeStationRepository.GetByGroupIdentifier(identifier);
                if (chargeStations.Any())
                {
                    foreach (var chargeStation in chargeStations)
                    {
                        await RemoveChargeStation(chargeStation.Identifier);
                    }
                }
                await GroupRepository.RemoveGroup(identifier);
                await SaveAsync();
            }
        }

        public async Task<(bool, float)> CheckIfCanAddToCurrentCapacityAndGetCurrentTotalMaxAmpsForGroup(Group group, float capacityToBeAdded)
        {
            var capacity = group.Capacity;
            var total = await GroupRepository.GetMaxCurrentInAmps(group);
            return (total + capacityToBeAdded <= capacity, total);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
