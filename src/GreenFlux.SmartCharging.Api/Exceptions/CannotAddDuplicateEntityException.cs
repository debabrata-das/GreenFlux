using System;

namespace GreenFlux.SmartCharging.Api.Mediators
{
    [Serializable]
    public class CannotAddDuplicateEntityException : Exception
    {
        public CannotAddDuplicateEntityException(string entityName, Guid identifier)
            : base($"Cannot add duplicate {entityName} with {identifier}")
        { }

        public CannotAddDuplicateEntityException(string entityName, int identifier)
            : base($"Cannot add duplicate {entityName} with {identifier}")
        { }
    }
}