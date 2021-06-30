using System;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Repository
{
    public interface IGroupRepository
    {
        Task AddGroup(Group group);
        Task<Group> GetByIdentifier(Guid identifier);
        Task RemoveGroup(Guid identifier);
        Task<float> GetMaxCurrentInAmps(Group group);
    }
}