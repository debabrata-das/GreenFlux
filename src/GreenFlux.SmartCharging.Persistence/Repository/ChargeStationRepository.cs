using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public class ChargeStationRepository : IChargeStationRepository
    {
        private readonly GreenFluxDbContext _context;

        public ChargeStationRepository(GreenFluxDbContext context)
        {
            _context = context;
        }

        public async Task Add(ChargeStation chargeStation)
        {
            await _context.ChargeStations.AddAsync(chargeStation);
        }

        public async Task<ChargeStation> GetByIdentifier(Guid identifier)
        {
            return await _context.ChargeStations.Include(cs => cs.Connectors)
                .FirstOrDefaultAsync(cs => cs.Identifier == identifier);
        }

        public async Task<float> GetMaxCurrentInAmps(ChargeStation chargeStation)
        {
            var chargeStationFromContext = await _context.ChargeStations.FindAsync(chargeStation.Identifier);
            if (chargeStationFromContext != null)
            {
                var csFound = await _context.ChargeStations.Include(cs => cs.Connectors)
                    .FirstOrDefaultAsync(cs => cs.Identifier == chargeStation.Identifier);
                if (csFound != null)
                {
                    return csFound.Connectors.Sum(c => c.MaxCurrentInAmps);
                }
            }

            return 0;
        }

        public async Task RemoveChargeStation(Guid identifier)
        {
            var item = await _context.ChargeStations.FindAsync(identifier);
            if (item != null)
            {
                await RemoveChargeStation(item);
            }
        }

        public async Task RemoveChargeStation(ChargeStation chargeStation)
        {
            _context.ChargeStations.Remove(chargeStation);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<ChargeStation>> GetByGroupIdentifier(Guid groupIdentifier)
        {
            return await _context.ChargeStations.Include(chargeStation => chargeStation.Group)
                .Where(chargeStation => chargeStation.GroupIdentifier == groupIdentifier).ToListAsync();
        }
    }
}