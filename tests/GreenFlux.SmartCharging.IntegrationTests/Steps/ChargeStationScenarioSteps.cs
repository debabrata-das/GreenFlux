using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.IntegrationTests.Helpers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace GreenFlux.SmartCharging.IntegrationTests.Steps
{
    [Binding]
    public class ChargeStationScenarioSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IntegrationTestsBase _integrationTestsBase;
        private ChargeStationDTO _chargeStation;

        public ChargeStationScenarioSteps(ScenarioContext scenarioContext, IntegrationTestsBase integrationTestsBase)
        {
            _scenarioContext = scenarioContext;
            _integrationTestsBase = integrationTestsBase;
            _chargeStation = new ChargeStationDTO();
        }

        [Given(@"the following ChargeStation details are available")]
        public void GivenTheFollowingChargeStationDetailsAreAvailable(Table table)
        {
            _chargeStation = table.CreateInstance<ChargeStationDTO>();
        }

        [When(@"the ChargeStation data is posted to the ChargeStation API endpoint")]
        public async Task ThenWhenTheChargeStationDataIsPostedToTheChargeStationApiEndpoint()
        {
            _scenarioContext["ChargeStation1"] = await _integrationTestsBase.CreateChargeStation(_chargeStation);
        }

        [Then(@"the ChargeStation should be saved successfully")]
        public async Task ThenTheChargeStationShouldBeSavedSuccessfully()
        {
            var httpResponseMessage = (HttpResponseMessage) _scenarioContext["ChargeStation1"];
            ChargeStationDTO output = await _integrationTestsBase.GetFromResponse<ChargeStationDTO>(httpResponseMessage);

            output.Identifier.Should().NotBeEmpty();
            output.Identifier.IsSameOrEqualTo(_chargeStation.Identifier);
            output.Name.IsSameOrEqualTo(_chargeStation.Name);
            output.GroupIdentifier.IsSameOrEqualTo(_chargeStation.GroupIdentifier);
        }
    }
}
