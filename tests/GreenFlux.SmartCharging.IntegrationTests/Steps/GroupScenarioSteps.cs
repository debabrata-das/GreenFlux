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
    public class GroupScenarioSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IntegrationTestsBase _integrationTestsBase;
        private GroupDTO _group;

        public GroupScenarioSteps(ScenarioContext scenarioContext, IntegrationTestsBase integrationTestsBase)
        {
            _scenarioContext = scenarioContext;
            _integrationTestsBase = integrationTestsBase;
            _group = new GroupDTO();
        }

        [Given(@"the following Group details are available")]
        public void GivenTheFollowingGroupDetailsAreAvailable(Table table)
        {
            _group = table.CreateInstance<GroupDTO>();
        }

        [When(@"the Group data is sent to the Group API endpoint")]
        public async Task ThenWhenTheGroupDataIsSentToTheGroupApiEndpoint()
        {
            _scenarioContext["Group1"] = await _integrationTestsBase.CreateGroup(_group);
        }

        [Then(@"the Group should be saved successfully")]
        public async Task ThenTheGroupShouldBeCreatedSuccessfully()
        {
            GroupDTO output = await _integrationTestsBase.GetFromResponse<GroupDTO>((HttpResponseMessage) _scenarioContext["Group1"]);

            output.Identifier.Should().NotBeEmpty();
            output.Identifier.IsSameOrEqualTo(_group.Identifier);
            output.Name.IsSameOrEqualTo(_group.Name);
            output.Capacity.IsSameOrEqualTo(_group.Capacity);
        }
    }
}
