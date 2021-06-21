using System;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Api;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Controllers;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Domain.Exceptions;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence;
using GreenFlux.SmartCharging.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GreenFlux.SmartCharging.UnitTests
{
    [TestFixture]
    public class ConnectorTests : TestBase
    {
        [Test]
        public async Task SaveConnectorToRepository_ShouldAddConnectorWithSameDetails()
        {
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(Guid.NewGuid(), guidChargeStation);
            var maxCurrentInAmps = 5;
            SaveConnectorOutput saveConnectorOutput = null;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };

            saveConnectorOutput = await SaveConnectorToRepository(saveConnector);

            var sut = saveConnectorOutput.ConnectorDto;
            Assert.IsNotNull(sut);
            Assert.That(sut.Identifier, Is.EqualTo(1));
            Assert.That(sut.MaxCurrentInAmps, Is.EqualTo(maxCurrentInAmps));
            Assert.That(sut.ChargeStationIdentifier, Is.EqualTo(guidChargeStation));
        }

        [Test]
        public async Task SaveConnectorWithInvalidData_ShouldThrowDataValidationException()
        {
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(Guid.NewGuid(), guidChargeStation);
            var maxCurrentInAmps = 5;
            SaveConnector saveConnector = new SaveConnector { Identifier = 10, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await SaveConnectorToRepository(saveConnector);
            });

            Assert.That(ex?.Message, Is.EqualTo("Connector validation failed"));
            Assert.That(ex?.InnerException, Is.InstanceOf(typeof(AggregateException)));
            Assert.That(ex?.InnerException?.InnerException?.Message, Is.EqualTo("For a Connector the only possible values for Identifier are - 1, 2, 3, 4, 5"));
        }

        [Test]
        public void SaveConnectorWithInvalidChargeStationIdentifier_ShouldThrowDataValidationException()
        {
            var maxCurrentInAmps = 5;
            var invalidChargeStationIdentifier = Guid.NewGuid();
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = invalidChargeStationIdentifier, MaxCurrentInAmps = maxCurrentInAmps };

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await SaveConnectorToRepository(saveConnector);
            });

            Assert.That(ex?.Message, Is.EqualTo($"Cannot create Connector since the Charge Station with Identifier '{invalidChargeStationIdentifier}' not found"));
        }

        [Test]
        public async Task GetConnector_ShouldGetConnectorWithSameDetails()
        {
            var connectorIdentifier = 1;
            var guidChargeStation = Guid.NewGuid();
            var maxCurrentInAmps = 5;
            await SaveConnector(guidChargeStation, connectorIdentifier, maxCurrentInAmps);
            ConnectorController connectorController = new ConnectorController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            var actionResult = await connectorController.GetConnector(connectorIdentifier, guidChargeStation);

            var connectorDto = (ConnectorDTO)((OkObjectResult)actionResult).Value;
            Assert.IsNotNull(connectorDto);
            Assert.That(connectorDto.Identifier, Is.EqualTo(connectorIdentifier));
            Assert.That(connectorDto.ChargeStationIdentifier, Is.EqualTo(guidChargeStation));
            Assert.That(connectorDto.MaxCurrentInAmps, Is.EqualTo(maxCurrentInAmps));
        }

        [Test]
        public async Task RemoveConnector_ShouldRemoveConnectorWithSameDetails()
        {
            var connectorIdentifier = 1;
            var guidChargeStation = Guid.NewGuid();
            var maxCurrentInAmps = 5;
            await SaveConnector(guidChargeStation, connectorIdentifier, maxCurrentInAmps);
            ConnectorController connectorController = new ConnectorController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            await connectorController.RemoveConnector(connectorIdentifier, guidChargeStation);

            var actionResult = await connectorController.GetConnector(connectorIdentifier, guidChargeStation);
            Assert.That(actionResult, Is.InstanceOf(typeof(NotFoundResult)));
        }

        private async Task SaveConnector(Guid guidChargeStation, int connectorIdentifier, int maxCurrentInAmps)
        {
            await SaveChargeStationToRepository(Guid.NewGuid(), guidChargeStation);
            SaveConnector saveConnector = new SaveConnector
            {
                Identifier = connectorIdentifier, ChargeStationIdentifier = guidChargeStation,
                MaxCurrentInAmps = maxCurrentInAmps
            };
            await SaveConnectorToRepository(saveConnector);
        }

        [Test]
        public void GetByIdentifierAndChargeStation_ShouldReturnConnectorFromRepositoryWithSameGuid()
        {
            var context = new GreenFluxDbContext(ContextOptions);
            var unitOfWork = new UnitOfWork(context);

            int connectorId = 5;
            var guid = Guid.NewGuid();

            ExecuteSync(async () =>
            {
                var demoConnector = GetDemoConnector(connectorId, 100,
                    GetDemoChargeStation("Demo Charge Station", guid, GetDemGroup("Demo Group", guid)));

                await unitOfWork.ConnectorRepository.AddConnector(demoConnector);
                await unitOfWork.SaveAsync();
            });

            var connector = ExecuteSync(() => unitOfWork.ConnectorRepository.GetByIdentifierAndChargeStation(connectorId, guid));

            Assert.IsNotNull(connector);
            Assert.That(connector.Identifier, Is.EqualTo(connectorId));
            Assert.That(connector.ChargeStation.Identifier, Is.EqualTo(guid));
            Assert.That(connector.ChargeStation.Name, Is.EqualTo("Demo Charge Station"));
            Assert.That(connector.ChargeStation.Group.Identifier, Is.EqualTo(guid));
            Assert.That(connector.ChargeStation.Group.Name, Is.EqualTo("Demo Group"));
        }

        [Test]
        public void SetInvalidIdentifierInConnector_ShouldThrowInvalidConnectorIdentifierException()
        {
            var connectorIdentifier = 6;
            Connector connector = new Connector();
            Assert.Throws<InvalidConnectorIdentifierException>(() =>
            {
                connector.Identifier = connectorIdentifier;
            });
        }
    }
}
