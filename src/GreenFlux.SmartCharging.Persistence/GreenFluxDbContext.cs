using System;
using System.Reflection;
using GreenFlux.SmartCharging.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GreenFlux.SmartCharging.Persistence
{
    public interface IGreenFluxDbContext : IDisposable
    {
        public DbSet<Connector> Connectors { get; set; }

        public DbSet<ChargeStation> ChargeStations { get; set; }

        public DbSet<Group> Groups { get; set; }
    }

    public class GreenFluxDbContext : DbContext, IGreenFluxDbContext
    {
        public GreenFluxDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Connector> Connectors { get; set; }

        public DbSet<ChargeStation> ChargeStations { get; set; }
        
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
