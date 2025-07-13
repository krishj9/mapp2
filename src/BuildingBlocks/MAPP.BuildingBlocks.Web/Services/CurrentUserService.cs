using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MAPP.BuildingBlocks.Application.Common.Interfaces;

namespace MAPP.BuildingBlocks.Web.Services;

/// <summary>
/// Current user service implementation for web applications
/// Adapted from Ardalis Clean Architecture template
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
