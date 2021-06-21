using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GreenFlux.SmartCharging.Api;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Controllers;
using GreenFlux.SmartCharging.Api.Exceptions;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence;
using GreenFlux.SmartCharging.Persistence.Exceptions;
using GreenFlux.SmartCharging.Persistence.Repository;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace GreenFlux.SmartCharging.UnitTests
{
    [TestFixture]
    public class GroupTests : TestBase
    {
        [Test]
        public async Task Handle_AddNewGroup_ShouldAddNewGroupToRepository()
        {
            var guid = Guid.NewGuid();
            SaveGroupHandler saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            SaveGroupOutput saveGroupOutput = await saveGroupHandler.Handle(new SaveGroup { Capacity = 100, Identifier = guid, Name = TestGroupName }, new CancellationToken(true));

            Assert.IsNotNull(saveGroupOutput);
            Assert.That(saveGroupOutput.GroupDto.Capacity, Is.EqualTo(100));
            Assert.That(saveGroupOutput.GroupDto.Identifier, Is.EqualTo(guid));
            Assert.That(saveGroupOutput.GroupDto.Name, Is.EqualTo(TestGroupName));
        }

        [Test]
        public void Handle_AddNewGroupWithInvalidCapacity_ShouldThrowDataValidationException()
        {
            var guid = Guid.NewGuid();
            var saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await saveGroupHandler.Handle(new SaveGroup { Capacity = 0, Identifier = guid, Name = TestGroupName }, new CancellationToken(true));
            });

            Assert.That(ex?.Message, Is.EqualTo("Group validation failed"));
            Assert.That(ex?.InnerException, Is.InstanceOf(typeof(AggregateException)));
            Assert.That(ex?.InnerException?.InnerException?.Message, Is.EqualTo("The capacity can not be null or empty"));
        }

        [Test]
        public void Handle_AddNewGroupWithInvalidData_ShouldThrowDataValidationException()
        {
            var guid = Guid.NewGuid();
            var saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());

            DataValidationException ex = Assert.ThrowsAsync<DataValidationException>(async () =>
            {
                await saveGroupHandler.Handle(new SaveGroup { Capacity = 0, Identifier = guid, Name = "" }, new CancellationToken(true));
            });

            var ae = (AggregateException)ex?.InnerException;
            ae?.Flatten();

            Assert.That(ex?.Message, Is.EqualTo("Group validation failed"));
            Assert.That(ex?.InnerException, Is.InstanceOf(typeof(AggregateException)));
            Assert.That(ae?.Message, Does.Contain("The Name value can not be null or empty"));
            Assert.That(ae?.Message, Does.Contain("The capacity can not be null or empty"));
            Assert.That(ae?.Message, Does.Contain("The capacity cannot be smaller than 0."));
        }

        [Test]
        public async Task AddDuplicateGroup_ShouldAddNewGroupToRepository()
        {
            var guid = Guid.NewGuid();
            SaveGroup saveGroup = new SaveGroup { Capacity = 100, Identifier = guid, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            CannotAddDuplicateEntityException ex = Assert.ThrowsAsync<CannotAddDuplicateEntityException>(async () =>
            {
                saveGroup.FromPost = true;
                await SaveGroupToRepository(saveGroup);
            });

            Assert.That(ex?.Message, Is.EqualTo($"Cannot add duplicate Group with {guid}"));
        }

        [Test]
        public async Task UpdateGroupCapacity_ShouldUpdateTheGroupCapacity()
        {
            var guid = Guid.NewGuid();
            SaveGroup saveGroup = new SaveGroup {Capacity = 10, Identifier = guid, Name = TestGroupName};
            await SaveGroupToRepository(saveGroup);

            saveGroup.Capacity = 100;
            var saveGroupOutput = await SaveGroupToRepository(saveGroup);

            Assert.IsNotNull(saveGroupOutput);
            Assert.That(saveGroupOutput.GroupDto.Capacity, Is.EqualTo(100));
        }

        [Test]
        public async Task DecreaseGroupCapacityToLowerThanCurrentAmpsTotal_ShouldThrowNewCapacityForGroupCannotBeLowerException()
        {
            var guid = Guid.NewGuid();
            SaveGroup saveGroup = new SaveGroup { Capacity = 100, Identifier = guid, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            var guidChargeStation = Guid.NewGuid();
            SaveChargeStation saveChargeStation = new SaveChargeStation {Identifier = guidChargeStation, GroupIdentifier = guid, Name = TestChargeStationName};
            await SaveChargeStationToRepository(saveChargeStation);

            var maxCurrentInAmps = 50;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            await SaveConnectorToRepository(saveConnector);

            int newCapacity = 10;
            saveGroup.Capacity = newCapacity;
            NewCapacityForGroupCannotBeLowerException ex = Assert.ThrowsAsync<NewCapacityForGroupCannotBeLowerException>(async () =>
            {
                await SaveGroupToRepository(saveGroup);
            });

            Assert.That(ex?.Message, Is.EqualTo(
                $"New Capacity of {newCapacity} for Group '{guid}' cannot be lower than the current available maximum current in Amps value of {maxCurrentInAmps}. " +
                $"Please use a value higher than {maxCurrentInAmps} as the new Capacity value."));
        }

        [Test]
        public async Task GetGroupByIdentifier_ShouldGetGroupWithSameDetails()
        {
            var identifier = Guid.NewGuid();
            var testGroupName = $"Test Group {Guid.NewGuid()}";
            SaveGroupHandler saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            await saveGroupHandler.Handle(new SaveGroup { Capacity = 100, Identifier = identifier, Name = testGroupName }, new CancellationToken(true));
            GroupController groupController = new GroupController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            var actionResult = await groupController.GetGroupByIdentifier(identifier);

            var groupDto = (GroupDTO)((OkObjectResult)actionResult).Value;
            Assert.That(groupDto.Identifier, Is.EqualTo(identifier));
            Assert.That(groupDto.Name, Is.EqualTo(testGroupName));
        }

        [Test]
        public async Task RemoveGroup_ShouldRemoveGroupWithSameIdentifier()
        {
            var guid = Guid.NewGuid();
            var testGroupName = $"Test Group {Guid.NewGuid()}";
            SaveGroupHandler saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            await saveGroupHandler.Handle(new SaveGroup { Capacity = 100, Identifier = guid, Name = testGroupName }, new CancellationToken(true));
            GroupController groupController = new GroupController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            await groupController.RemoveGroup(guid);

            var actionResult = await groupController.GetGroupByIdentifier(guid);
            Assert.That(actionResult, Is.InstanceOf(typeof(NotFoundResult)));
        }

        [Test]
        public async Task GetTotalCurrentInAmps_ShouldGetTotalCurrentInAmpsFromChargeStation()
        {
            var guidGroup = Guid.NewGuid();
            var guidChargeStation = Guid.NewGuid();
            await SaveChargeStationToRepository(guidGroup, guidChargeStation);
            var maxCurrentInAmps = 5;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            await SaveConnectorToRepository(saveConnector);

            GroupController groupController =
                new GroupController(GetUnitOfWork(), new LoggerManager(), GetMapper(), null);

            var actionResult = await groupController.GetTotalCurrentInAmps(guidGroup);

            var ampsValue = (float)((OkObjectResult)actionResult).Value;
            Assert.That(ampsValue, Is.EqualTo(maxCurrentInAmps));
        }

        [Test]
        public async Task GetMaxCurrentInAmps_ShouldBeLowerThanGroupCapacity()
        {
            var guid = Guid.NewGuid();
            var groupCapacity = 100;
            SaveGroup saveGroup = new SaveGroup { Capacity = groupCapacity, Identifier = guid, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            var guidChargeStation = Guid.NewGuid();
            SaveChargeStation saveChargeStation = new SaveChargeStation { Identifier = guidChargeStation, GroupIdentifier = guid, Name = TestChargeStationName };
            await SaveChargeStationToRepository(saveChargeStation);

            var maxCurrentInAmps = 50;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            await SaveConnectorToRepository(saveConnector);

            var group = await GetUnitOfWork().GroupRepository.GetByIdentifier(guid);
            var maxCurrentInAmpsForGroup = await GetUnitOfWork().GroupRepository.GetMaxCurrentInAmps(group);

            Assert.That(maxCurrentInAmpsForGroup, Is.LessThanOrEqualTo(groupCapacity));
        }

        [Test]
        public async Task AddConnectorWithAmpsGreaterThanGroupCapacity_ShouldThrowConnectorCapacityExceededForGroupException()
        {
            var guid = Guid.NewGuid();
            var groupCapacity = 50;
            SaveGroup saveGroup = new SaveGroup { Capacity = groupCapacity, Identifier = guid, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            var guidChargeStation = Guid.NewGuid();
            SaveChargeStation saveChargeStation = new SaveChargeStation { Identifier = guidChargeStation, GroupIdentifier = guid, Name = TestChargeStationName };
            await SaveChargeStationToRepository(saveChargeStation);

            var maxCurrentInAmps = 100;
            SaveConnector saveConnector = new SaveConnector { Identifier = 1, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };

            Assert.ThrowsAsync<ConnectorCapacityExceededForGroupException>(async () =>
            {
                await SaveConnectorToRepository(saveConnector);
            });
        }

        [Test]
        public async Task AddSecondConnectorWithCombinedAmpsGreaterThanGroupCapacity_ShouldThrowConnectorCapacityExceededForGroupException()
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
            saveConnector = new SaveConnector { Identifier = 2, ChargeStationIdentifier = guidChargeStation, MaxCurrentInAmps = maxCurrentInAmps };
            Assert.ThrowsAsync<ConnectorCapacityExceededForGroupException>(async () =>
            {
                await SaveConnectorToRepository(saveConnector);
            });
        }

        [Test]
        public async Task GetByIdentifier_ShouldReturnGroupWithSameDetails()
        {
            var context = new GreenFluxDbContext(ContextOptions);
            var unitOfWork = new UnitOfWork(context);

            var guid = Guid.NewGuid();

            await unitOfWork.GroupRepository.AddGroup(GetDemGroup(TestGroupName, guid));
            await unitOfWork.SaveAsync();

            Group sut = await unitOfWork.GroupRepository.GetByIdentifier(guid);

            Assert.IsNotNull(sut);
            Assert.That(sut.Name, Is.EqualTo(TestGroupName));
            Assert.That(sut.Identifier, Is.EqualTo(guid));
        }

        [Test]
        public void Map_MapGroupToGroupDTO_ShouldReturnGroupDToWithSameDetails()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DtoMappingProfile>());
            var mapper = config.CreateMapper();

            var guid = Guid.NewGuid();
            Group source = new Group {Capacity = 100, Identifier = guid, Name = TestGroupName};
            GroupDTO groupDto = mapper.Map<GroupDTO>(source);

            Assert.That(groupDto.Name, Is.EqualTo(source.Name));
            Assert.That(groupDto.Capacity, Is.EqualTo(source.Capacity));
            Assert.That(groupDto.Identifier, Is.EqualTo(source.Identifier));
        }
    }
}
