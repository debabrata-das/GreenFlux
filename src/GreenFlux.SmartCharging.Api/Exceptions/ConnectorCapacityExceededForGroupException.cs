using System;

namespace GreenFlux.SmartCharging.Api.Exceptions
{
    [Serializable]
    public class ConnectorCapacityExceededForGroupException : Exception
    {
        public ConnectorCapacityExceededForGroupException(float capacity, float total, float maxCurrentInAmps)
            : base($"Cannot add this Connector since adding 'MaxCurrentInAmps' value of {maxCurrentInAmps} to the " +
                   $"current total capacity of {total} will exceed the Group capacity of {capacity}. Lower the 'MaxCurrentInAmps' and then retry, or remove another Connector and then retry.")
        {
        }
    }
}