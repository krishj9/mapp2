using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Infrastructure.Data;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;
using MAPP.Modules.Planning.Application.Common.Interfaces;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Infrastructure.Data;

/// <summary>
/// Planning database context following clean architecture patterns
/// </summary>
public class PlanningDbContext : BaseDbContext, IPlanningDbContext
{
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanItem> PlanItems => Set<PlanItem>();

    public PlanningDbContext(
        DbContextOptions<PlanningDbContext> options,
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
