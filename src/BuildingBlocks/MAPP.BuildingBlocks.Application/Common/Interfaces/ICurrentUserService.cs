namespace MAPP.BuildingBlocks.Application.Common.Interfaces;

/// <summary>
/// Service for accessing current user information
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public interface ICurrentUserService
{
    string? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
