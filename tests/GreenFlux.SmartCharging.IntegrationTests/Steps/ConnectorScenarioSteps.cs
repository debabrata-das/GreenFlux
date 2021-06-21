using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions.Common;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.IntegrationTests.Helpers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace GreenFlux.SmartCharging.IntegrationTests.Steps
{
    [Binding]
    public class ConnectorScenarioSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IntegrationTestsBase _integrationTestsBase;
        private ConnectorDTO _connector;

        public ConnectorScenarioSteps(ScenarioContext scenarioContext, IntegrationTestsBase integrationTestsBase)
        {
            _scenarioContext = scenarioContext;
            _integrationTestsBase = integrationTestsBase;
            _connector = new ConnectorDTO();
        }

        [Given(@"the following Connector details are available")]
        public void GivenTheFollowingConnectorDetailsAreAvailable(Table table)
        {
            _connector = table.CreateInstance<ConnectorDTO>();
        }

        [When(@"the Connector data is posted to the ChargeStation API endpoint")]
        public async Task ThenWhenTheConnectorDataIsPostedToTheChargeStationApiEndpoint()
        {
            _scenarioContext["Connector1"] = await _integrationTestsBase.CreateConnector(_connector);
        }

        [Then(@"the Connector should be saved successfully")]
        public async Task ThenTheConnectorShouldBeSavedSuccessfully()
        {
            var httpResponseMessage = (HttpResponseMessage)_scenarioContext["Connector1"];
            ConnectorDTO output = await _integrationTestsBase.GetFromResponse<ConnectorDTO>(httpResponseMessage);
            
            output.Identifier.IsSameOrEqualTo(_connector.Identifier);
            output.ChargeStationIdentifier.IsSameOrEqualTo(_connector.ChargeStationIdentifier);
            output.MaxCurrentInAmps.IsSameOrEqualTo(_connector.MaxCurrentInAmps);
        }
    }
}
