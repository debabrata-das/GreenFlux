using System;

namespace GreenFlux.SmartCharging.Domain.Exceptions
{
    public class InvalidConnectorIdentifierException : ApplicationException
    {
        public InvalidConnectorIdentifierException(int identifier)
        : base($@"Provided value {identifier} is invalid. Please provide a value between 1 & 5.")
        {
           
        }
    }
}