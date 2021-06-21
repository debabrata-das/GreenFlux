using System;

namespace GreenFlux.SmartCharging.Api.Exceptions
{
    [Serializable]
    public class DataValidationException : Exception
    {
        public DataValidationException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}