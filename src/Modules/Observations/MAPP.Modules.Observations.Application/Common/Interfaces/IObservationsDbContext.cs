using Microsoft.EntityFrameworkCore;
using MAPP.BuildingBlocks.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Application.Common.Interfaces;

/// <summary>
/// Observations database context interface
/// Following Ardalis Clean Architecture patterns
/// </summary>
public interface IObservationsDbContext : IApplicationDbContext
{
    DbSet<Observation> Observations { get; }
    DbSet<ObservationData> ObservationData { get; }
}
