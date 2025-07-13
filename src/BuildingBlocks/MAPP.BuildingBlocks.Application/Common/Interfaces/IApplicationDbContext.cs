namespace MAPP.BuildingBlocks.Application.Common.Interfaces;

/// <summary>
/// Base interface for application database contexts
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
