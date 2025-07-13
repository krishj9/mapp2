using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.BuildingBlocks.Infrastructure.Data.Interceptors;

namespace MAPP.BuildingBlocks.Infrastructure.Data;

/// <summary>
/// Base database context adapted from Ardalis Clean Architecture template
/// Provides common functionality for all domain contexts
/// </summary>
public abstract class BaseDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    protected BaseDbContext(
        DbContextOptions options,
        IDomainEventService domainEventService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _domainEventService = domainEventService;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _domainEventService.PublishAsync(domainEvent);
    }
}
