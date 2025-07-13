using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Data;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data;

/// <summary>
/// Observations database context following Ardalis patterns
/// </summary>
public class ObservationsDbContext : BaseDbContext, IObservationsDbContext
{
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<ObservationData> ObservationData => Set<ObservationData>();

    public ObservationsDbContext(
        DbContextOptions<ObservationsDbContext> options,
        IDomainEventService domainEventService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options, domainEventService, auditableEntitySaveChangesInterceptor)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}
