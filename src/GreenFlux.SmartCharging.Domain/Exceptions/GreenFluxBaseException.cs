using System;

namespace GreenFlux.SmartCharging.Domain.Exceptions
{
    [Serializable]
    public class GreenFluxBaseException : ApplicationException
    {
        public GreenFluxBaseException(string message, Exception exception)
            : base(message, exception)
        {
            
        }

        public GreenFluxBaseException(string message)
            : base(message)
        {

        }
    }
}