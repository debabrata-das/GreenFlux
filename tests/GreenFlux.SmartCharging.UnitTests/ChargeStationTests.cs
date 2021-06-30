using System;
using System.Threading;
using System.Threading.Tasks;
using GreenFlux.SmartCharging.Api;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Controllers;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Persistence;
using GreenFlux.SmartCharging.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GreenFlux.SmartCharging.UnitTests
{
    [TestFixture]
    public class ChargeStationTests : TestBase
    {
        [Test]
        public async Task SaveChargeStationToRepository_ShouldAddChargeStationWithSameDetails()
        {
            var guid = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();

            var chargeStation = await SaveChargeStationToRepository(guid, guidChargeStation);
            
            Assert.IsNotNull(chargeStation);
            Assert.That(chargeStation.Identifier, Is.EqualTo(guidChargeStation));
            Assert.That(chargeStation.Name, Is.EqualTo(TestChargeStationName));
            Assert.That(chargeStation.GroupIdentifier, Is.EqualTo(guid));
        }

        [Test]
        public async Task GetChargeStationByIdentifier_ShouldGetChargeStationWithSameDetails()
        {
            var guidGroup = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(guidGroup, guidChargeStation, "Test ChargeStation 1");
            ChargeStationController chargeStationController =
                new ChargeStationController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            var actionResult = await chargeStationController.GetChargeStationByIdentifier(guidChargeStation);

            var chargeStationDto = ((ChargeStationDTO)((OkObjectResult) actionResult.Result).Value);
            Assert.That(chargeStationDto.Identifier, Is.EqualTo(guidChargeStation));
            Assert.That(chargeStationDto.GroupIdentifier, Is.EqualTo(guidGroup));
            Assert.That(chargeStationDto.Name, Is.EqualTo("Test ChargeStation 1"));
        }

        [Test]
        public async Task RemoveChargeStation_ShouldRemoveChargeStationWithSameIdentifier()
        {
            var guid = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(guid, guidChargeStation);
            ChargeStationController chargeStationController =
                new ChargeStationController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            await chargeStationController.RemoveChargeStation(guidChargeStation);

            var actionResult = await chargeStationController.GetChargeStationByIdentifier(guidChargeStation);
            Assert.That(actionResult.Result, Is.InstanceOf(typeof(NotFoundResult)));
        }

        [Test]
        public async Task GetTotalCurrentInAmps_ShouldGetTotalCurrentInAmpsFromChargeStation()
        {
            var guid = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(guid, guidChargeStation);
            var maxCurrentInAmps = 5;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            await SaveConnectorToRepository(saveConnector);
            ChargeStationController chargeStationController =
                new ChargeStationController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            var actionResult = await chargeStationController.GetTotalCurrentInAmps(guidChargeStation);

            var ampsValue = (float)((OkObjectResult)actionResult.Result).Value;
            Assert.That(ampsValue, Is.EqualTo(maxCurrentInAmps));
        }

        [Test]
        public async Task SaveChargeStationWithConnector_ShouldAddConnectorHavingSameDetails()
        {
            var guid = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();

            await SaveChargeStationToRepository(guid, guidChargeStation);

            // Save Connector 1
            var maxCurrentInAmps = 5;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            var saveConnectorOutput = await SaveConnectorToRepository(saveConnector);

            var connector = saveConnectorOutput.ConnectorDto;

            Assert.IsNotNull(connector);
            Assert.That(connector.Identifier, Is.EqualTo(1));
            Assert.That(connector.ChargeStationIdentifier, Is.EqualTo(guidChargeStation));
            Assert.That(connector.MaxCurrentInAmps, Is.EqualTo(maxCurrentInAmps));
        }

        [Test]
        public void AddNewChargeStationWithInvalidData_ShouldThrowDataValidationException()
        {
            var guid = Guid.NewGuid();
            var saveChargeStationHandler= new SaveChargeStationHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await saveChargeStationHandler.Handle(new SaveChargeStation() { Identifier = guid, Name = "" }, new CancellationToken(true));
            });

            Assert.That(ex?.Message, Is.EqualTo("Charge Station validation failed"));
            Assert.That(ex?.InnerException, Is.InstanceOf(typeof(AggregateException)));
            Assert.That(ex?.InnerException?.InnerException?.Message, Is.EqualTo("The name can not be null or empty"));
        }

        [Test]
        public void AddNewChargeStationWithInvalidGroup_ShouldThrowDataValidationException()
        {
            var invalidGroupIdentifier = Guid.NewGuid();
            var guid = Guid.NewGuid();
            var saveChargeStationHandler = new SaveChargeStationHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await saveChargeStationHandler.Handle(new SaveChargeStation() { Identifier = guid, Name = TestChargeStationName, GroupIdentifier = invalidGroupIdentifier}, new CancellationToken(true));
            });

            Assert.That(ex?.Message, Is.EqualTo($"Cannot create Charge Station since the Group with Identifier '{invalidGroupIdentifier}' not found"));
        }

        [Test]
        public async Task Handle_UpdateChargeStationDetails_ShouldGetChargeStationWithSameDetails()
        {
            var guidGroup = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(guidGroup, guidChargeStation);
            var saveChargeStationHandler = new SaveChargeStationHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            var chargeStationNewName = $"{TestChargeStationName}-{Guid.NewGuid()}";
            var chargeStationOutput = await saveChargeStationHandler.Handle(new SaveChargeStation() { Identifier = guidGroup, Name = chargeStationNewName, GroupIdentifier = guidGroup }, new CancellationToken(true));

            var chargeStationDto = chargeStationOutput.ChargeStationDto;
            Assert.IsNotNull(chargeStationDto);
            Assert.That(chargeStationDto.Name, Is.EqualTo(chargeStationNewName));
        }

        [Test]
        public async Task AddDuplicateChargeStation_ShouldThrowCannotAddDuplicateEntityException()
        {
            var guidGroup = Guid.NewGuid();
            var groupCapacity = 10;
            SaveGroup saveGroup = new SaveGroup { Capacity = groupCapacity, Identifier = guidGroup, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            var saveChargeStationHandler = new SaveChargeStationHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            var guid = Guid.NewGuid();

            var saveChargeStation = new SaveChargeStation() { Identifier = guid, Name = TestChargeStationName, GroupIdentifier = guidGroup };
            await saveChargeStationHandler.Handle(saveChargeStation, new CancellationToken(true));

            CannotAddDuplicateEntityException exc = Assert.ThrowsAsync<CannotAddDuplicateEntityException>(async () =>
            {
                saveChargeStation.FromPost = true;
                await saveChargeStationHandler.Handle(saveChargeStation, new CancellationToken(true));
            });

            Assert.IsNotNull(exc);
            Assert.That(exc.Message, Is.EqualTo($"Cannot add duplicate ChargeStation with {guid}"));
        }

        [Test]
        public async Task AddSecondConnectorWithSameIdentifier_ShouldThrowCannotAddDuplicateEntityException()
        {
            var guid = Guid.NewGuid();
            var groupCapacity = 10;
            SaveGroup saveGroup = new SaveGroup { Capacity = groupCapacity, Identifier = guid, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            var guidChargeStation = Guid.NewGuid();
            SaveChargeStation saveChargeStation = new SaveChargeStation { Identifier = guidChargeStation, GroupIdentifier = guid, Name = TestChargeStationName };
            await SaveChargeStationToRepository(saveChargeStation);

            // Save Connector 1
            var maxCurrentInAmps = 5;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            Assert.DoesNotThrowAsync(async () => await SaveConnectorToRepository(saveConnector));

            // Save Connector 2
            maxCurrentInAmps = 6;
            saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            saveConnector.FromPost = true;
            Assert.ThrowsAsync<CannotAddDuplicateEntityException>(async () =>
            {
                await SaveConnectorToRepository(saveConnector);
            });
        }

        [Test]
        public void GetByIdentifier_ShouldReturnConnectorFromRepositoryWithSameDetails()
        {
            var context = new GreenFluxDbContext(ContextOptions);
            var unitOfWork = new UnitOfWork(context);

            var guid = Guid.NewGuid();

            ExecuteSync(async () =>
            {
                await unitOfWork.ChargeStationRepository.Add(GetDemoChargeStation(TestChargeStationName, guid, GetDemGroup(TestChargeStationName, guid)));
                await unitOfWork.SaveAsync();
            });

            var sut = ExecuteSync(() => unitOfWork.ChargeStationRepository.GetByIdentifier(guid));

            Assert.IsNotNull(sut);
            Assert.That(sut.Identifier, Is.EqualTo(guid));
            Assert.That(sut.Name, Is.EqualTo(TestChargeStationName));
            Assert.That(sut.GroupIdentifier, Is.EqualTo(guid));
        }
    }
}
