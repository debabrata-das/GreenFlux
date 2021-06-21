using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public class ConnectorRepository : IConnectorRepository
    {
        private readonly GreenFluxDbContext _context;

        public ConnectorRepository(GreenFluxDbContext context)
        {
            _context = context;
        }

        public async Task AddConnector(Connector connector)
        {
            await _context.Connectors.AddAsync(connector);
            await _context.SaveChangesAsync();
        }

        public async Task<Connector> GetByIdentifierAndChargeStation(int identifier, Guid chargeStationIdentifier)
        {
            return await _context.Connectors.FindAsync(identifier, chargeStationIdentifier);
        }
        
        public async Task RemoveConnectorByIdentifierAndChargeStation(int identifier, Guid chargeStationIdentifier)
        {
            Connector connector = await _context.Connectors.FindAsync(identifier, chargeStationIdentifier);
            if (connector != null)
            {
                await RemoveConnector(connector);
            }
        }

        public async Task RemoveConnector(Connector connector)
        {
            _context.Connectors.Remove(connector);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<Connector>> GetByChargeStationIdentifier(Guid identifier)
        {
            return await _context.Connectors.Include(connector => connector.ChargeStation)
                .Where(connector => connector.ChargeStationIdentifier == identifier).ToListAsync();
        }
    }
}