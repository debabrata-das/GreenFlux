namespace GreenFlux.SmartCharging.Api.Controllers
{
    public static class Routes
    {
        public const string GroupId = "{groupId}";
        public const string ChargeStationId = "{chargeStationId}";
        public const string ConnectorId = "{connectorId}";

        public const string GroupsPath = "api/Groups";
        public const string ChargeStationsPath = "api/Groups/{groupId}/ChargeStations";
        public const string ConnectorsPath = "api/Groups/{groupId}/ChargeStations/{chargeStationId}/Connectors";
    }
}
