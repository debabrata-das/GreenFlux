using GreenFlux.SmartCharging.Persistence;
using GreenFlux.SmartCharging.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GreenFlux.SmartCharging.Api
{
    public static class ServiceExtensions
    {
        public static void CorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void PersistenceConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IChargeStationRepository, ChargeStationRepository>();
            services.AddScoped<IConnectorRepository, ConnectorRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<GreenFluxDbContext>(opts => opts.UseInMemoryDatabase(databaseName: "GreenFlux"));
        }
    }
}
