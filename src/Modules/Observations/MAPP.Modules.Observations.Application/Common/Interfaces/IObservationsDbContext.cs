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
    DbSet<ObservationArtifact> ObservationArtifacts { get; }
    DbSet<ObservationDomain> ObservationDomains { get; }
    DbSet<ObservationAttribute> ObservationAttributes { get; }
    DbSet<ProgressionPoint> ProgressionPoints { get; }
}
