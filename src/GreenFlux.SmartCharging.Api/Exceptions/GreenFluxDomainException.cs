using System;

namespace GreenFlux.SmartCharging.Api.Exceptions
{
    [Serializable]
    public class GreenFluxDomainException : Exception
    {
        public GreenFluxDomainException(string message, Exception exception)
            : base(message, exception)
        {
            
        }
    }
}