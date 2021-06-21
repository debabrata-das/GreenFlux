using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using GreenFlux.SmartCharging.Api;
using GreenFlux.SmartCharging.Api.AutoMapper;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace GreenFlux.SmartCharging.IntegrationTests.Helpers
{
    public class IntegrationTestsBase
    {
        private static string GroupApiName => "Group/";
        private static string ChargeStationApiName => "ChargeStation/";
        private static string ConnectorApiName => "Connector/";
        public string GroupEndpoint { get; }
        public string ChargeStationEndpoint { get; }
        public string ConnectorEndpoint { get; }
        public string GreenFluxApiBaseUrl { get; set; }
        public HttpClient Client { get; set; }

        protected IntegrationTestsBase()
        {
            WebApplicationFactory<Startup> webApplicationFactory = new WebApplicationFactory<Startup>();
            Client = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
            GreenFluxApiBaseUrl = $"{Client?.BaseAddress?.AbsoluteUri}api/";
            GroupEndpoint = GreenFluxApiBaseUrl + GroupApiName;
            ChargeStationEndpoint = GreenFluxApiBaseUrl + ChargeStationApiName;
            ConnectorEndpoint = GreenFluxApiBaseUrl + ConnectorApiName;
        }

        public T ToObject<T>(string json) => JsonConvert.DeserializeObject<T>(json);

        public StringContent ToStringContent<T>(T data) => new(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        public async Task<HttpResponseMessage> CreateGroup(GroupDTO groupDto)
        {
            return await Client.PostAsync(GroupEndpoint, ToStringContent(groupDto));
        }

        public async Task<HttpResponseMessage> CreateChargeStation(ChargeStationDTO chargeStationDto)
        {
            return await Client.PostAsync(ChargeStationEndpoint, ToStringContent(chargeStationDto));
        }

        public async Task<HttpResponseMessage> CreateConnector(ConnectorDTO connectorDto)
        {
            return await Client.PostAsync(GroupEndpoint, ToStringContent(connectorDto));
        }

        public async Task<T> GetFromResponse<T>(HttpResponseMessage response)
        {
           var conAsStringAsync = await response.Content.ReadAsStringAsync();
           Func<T> func = () => ToObject<T>(conAsStringAsync);
           func.Should().NotThrow();
           return func();
        }
    }
}