using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.Modules.Planning.Domain.Entities;

namespace MAPP.Modules.Planning.Application.Common.Interfaces;

/// <summary>
/// Planning database context interface
/// Following Ardalis Clean Architecture patterns
/// </summary>
public interface IPlanningDbContext : IApplicationDbContext
{
    DbSet<Plan> Plans { get; }
    DbSet<PlanItem> PlanItems { get; }
}
