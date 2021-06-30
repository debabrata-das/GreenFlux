using System;
using System.Linq;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly GreenFluxDbContext _context;

        public GroupRepository(GreenFluxDbContext context)
        {
            _context = context;
        }

        public async Task AddGroup(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task<Group> GetByIdentifier(Guid identifier)
        {
            var groupFromContext = await _context.Groups.FindAsync(identifier);
            if (groupFromContext != null)
            {
                return await _context.Groups.Include(group => group.ChargeStations)
                    .ThenInclude(chargeStation => chargeStation.Connectors)
                    .FirstOrDefaultAsync(group => group.Identifier == identifier);
            }

            return null;
        }

        public async Task RemoveGroup(Guid identifier)
        {
            Group group = await _context.Groups.FindAsync(identifier);
            if (group != null)
            {
                _context.Groups.Remove(group);
                var connectors = _context.Connectors.Where(c => c.ChargeStationIdentifier == identifier);
                foreach (var connector in connectors)
                {
                    _context.Connectors.Remove(connector);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<float> GetMaxCurrentInAmps(Group group)
        {
            var groupFromContext = await GetByIdentifier(group.Identifier);
            if (groupFromContext != null)
            {
                return groupFromContext.ChargeStations.Sum(chargeStation => chargeStation.Connectors.Sum(connector => connector.MaxCurrentInAmps));
            }

            return 0;
        }
    }
}