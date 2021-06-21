using System;
using GreenFlux.SmartCharging.Domain.Models;

namespace GreenFlux.SmartCharging.Persistence.Exceptions
{
    [Serializable]
    public class NewCapacityForGroupCannotBeLowerException : Exception
    {
        public NewCapacityForGroupCannotBeLowerException(Group existingGroup, float newCapacity, float currentAvailableCurrentInAmps)
            :base($"New Capacity of {newCapacity} for Group '{existingGroup.Identifier}' cannot be lower than the current available maximum current in Amps value of {currentAvailableCurrentInAmps}. " +
                  $"Please use a value higher than {currentAvailableCurrentInAmps} as the new Capacity value.")
        {
            
        }
    }
}