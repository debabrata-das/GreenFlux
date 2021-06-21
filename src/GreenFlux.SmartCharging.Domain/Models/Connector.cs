using System;
using System.Collections.Generic;
using GreenFlux.SmartCharging.Domain.Exceptions;

namespace GreenFlux.SmartCharging.Domain.Models
{
    public class Connector
    {
        private ChargeStation _chargeStation;
        private int _identifier;

        public int Identifier
        {
            get => _identifier;
            set
            {
                if (value is < 1 or > 5)
                {
                    throw new InvalidConnectorIdentifierException(value);
                }

                _identifier = value;
            }
        }

        public Guid ChargeStationIdentifier { get; set; }

        public ChargeStation ChargeStation
        {
            get => _chargeStation;
            set
            {
                _chargeStation = value;
                if (_chargeStation != null)
                {
                    ChargeStationIdentifier = _chargeStation.Identifier;
                }
            }
        }

        public float MaxCurrentInAmps { get; set; }

        public Connector() { }
    }

    public class ConnectorComparer : IEqualityComparer<Connector>
    {
        public bool Equals(Connector connector, Connector otherConnector)
        {
            if (connector == null && otherConnector == null)
            {
                return true;
            }

            if (connector == null || otherConnector == null)
            {
                return false;
            }

            return connector.Identifier.Equals(otherConnector.Identifier);
        }

        public int GetHashCode(Connector connector)
        {
            return connector.Identifier.GetHashCode();
        }
    }
}
