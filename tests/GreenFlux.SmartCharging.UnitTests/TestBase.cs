using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GreenFlux.SmartCharging.Api;
using GreenFlux.SmartCharging.Api.AutoMapper;
using GreenFlux.SmartCharging.Api.Mediators;
using GreenFlux.SmartCharging.Domain.Models;
using GreenFlux.SmartCharging.Persistence;
using GreenFlux.SmartCharging.Persistence.Repository;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.UnitTests
{
    public abstract class TestBase
    {
        private static readonly TaskFactory MyTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
        public const string TestGroupName = "Test Group 1";
        public const string TestChargeStationName = "Test CS 1";
        public DbContextOptions<GreenFluxDbContext> ContextOptions;

        protected TestBase()
        {
            ContextOptions
                = new DbContextOptionsBuilder<GreenFluxDbContext>()
                    .UseInMemoryDatabase(databaseName: "UnitTestGreenFlux")
                    .Options;

            using (var context = new GreenFluxDbContext(ContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }

        public async Task<SaveGroupOutput> SaveGroupToRepository(SaveGroup saveGroup)
        {
            var saveGroupHandler = new SaveGroupHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            return await saveGroupHandler.Handle(saveGroup, new CancellationToken(true));
        }

        public async Task<SaveConnectorOutput> SaveConnectorToRepository(SaveConnector saveConnector)
        {
            var saveConnectorHandler = new SaveConnectorHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            return await saveConnectorHandler.Handle(saveConnector, new CancellationToken(true));
        }

        public async Task<ChargeStationDTO> SaveChargeStationToRepository(Guid guidGroup, Guid guidChargeStation, string chargeStationName = TestChargeStationName)
        {
            var groupCapacity = 10;
            SaveGroup saveGroup = new SaveGroup { Capacity = groupCapacity, Identifier = guidGroup, Name = TestGroupName };
            await SaveGroupToRepository(saveGroup);

            SaveChargeStation saveChargeStation = new SaveChargeStation { Identifier = guidChargeStation, GroupIdentifier = guidGroup, Name = chargeStationName };
            var saveChargeStationOutput = await SaveChargeStationToRepository(saveChargeStation);
            var chargeStation = saveChargeStationOutput.ChargeStationDto;
            return chargeStation;
        }

        public async Task<SaveChargeStationOutput> SaveChargeStationToRepository(SaveChargeStation saveChargeStation)
        {
            var saveChargeStationHandler = new SaveChargeStationHandler(GetUnitOfWork(), GetMapper(), new LoggerManager());
            return await saveChargeStationHandler.Handle(saveChargeStation, new CancellationToken(true));
        }

        public static TResult ExecuteSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory
                .StartNew<Task<TResult>>(func)
                .Unwrap<TResult>()
                .GetAwaiter()
                .GetResult();
        }

        public static void ExecuteSync(Func<Task> func)
        {
            MyTaskFactory
                .StartNew<Task>(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        public Connector GetDemoConnector(int identifier, float maxCurrentInAmps, ChargeStation chargeStation)
        {
            return new Connector() { Identifier = identifier, MaxCurrentInAmps = maxCurrentInAmps, ChargeStation = chargeStation };
        }

        public ChargeStation GetDemoChargeStation(string name, Guid guid, Group group)
        {
            return new ChargeStation() { Name = name, Identifier = guid, Group = group };
        }

        public Group GetDemGroup(string name, Guid guid)
        {
            return new Group() { Name = name, Identifier = guid, Capacity = 100 };
        }

        public static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DtoMappingProfile>());
            var mapper = config.CreateMapper();
            return mapper;
        }

        public UnitOfWork GetUnitOfWork()
        {
            var context = new GreenFluxDbContext(ContextOptions);
            var unitOfWork = new UnitOfWork(context);
            return unitOfWork;
        }
    }
}
