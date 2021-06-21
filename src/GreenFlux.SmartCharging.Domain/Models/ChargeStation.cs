using System;
using System.Collections.Generic;

namespace GreenFlux.SmartCharging.Domain.Models
{
    public class ChargeStation
    {
        private readonly HashSet<int> _currentChargeStationsConnectorIdentifiers;

        private Group _group;

        public Guid Identifier { get; set; }

        public Guid GroupIdentifier { get; set; }

        public string Name { get; set; }

        public Group Group
        {
            get => _group;
            set
            {
                _group = value;
                if (_group != null)
                {
                    GroupIdentifier = _group.Identifier;
                }
            }
        }

        public HashSet<Connector> Connectors { get; set; }

        public ChargeStation()
        {
            Connectors = new HashSet<Connector>(new ConnectorComparer());
            _currentChargeStationsConnectorIdentifiers = new HashSet<int>();
        }
    }

    public class ChargeStationComparer : IEqualityComparer<ChargeStation>
    {
        public bool Equals(ChargeStation chargeStation, ChargeStation otherChargeStation)
        {
            if (chargeStation == null && otherChargeStation == null)
            {
                return true;
            }

            if (chargeStation == null || otherChargeStation == null)
            {
                return false;
            }

            return chargeStation.Identifier.Equals(otherChargeStation.Identifier);
        }

        public int GetHashCode(ChargeStation chargeStation)
        {
            return chargeStation.Identifier.GetHashCode();
        }
    }
}

