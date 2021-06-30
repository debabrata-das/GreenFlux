using System;
using GreenFlux.SmartCharging.Domain.Exceptions;

namespace GreenFlux.SmartCharging.Api.Exceptions
{
    [Serializable]
    public class DataValidationException : GreenFluxBaseException
    {
        public DataValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}