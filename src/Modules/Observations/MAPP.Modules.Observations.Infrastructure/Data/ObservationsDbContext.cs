using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Data;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Infrastructure.Data;

/// <summary>
/// Observations database context following clean architecture patterns
/// </summary>
public class ObservationsDbContext : BaseDbContext, IObservationsDbContext
{
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<ObservationArtifact> ObservationArtifacts => Set<ObservationArtifact>();
    public DbSet<ObservationDomain> ObservationDomains => Set<ObservationDomain>();
    public DbSet<ObservationAttribute> ObservationAttributes => Set<ObservationAttribute>();
    public DbSet<ProgressionPoint> ProgressionPoints => Set<ProgressionPoint>();

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
