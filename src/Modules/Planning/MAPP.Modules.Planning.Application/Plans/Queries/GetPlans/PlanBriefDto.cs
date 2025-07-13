using AutoMapper;
using MAPP.Modules.Planning.Domain.Entities;
using MAPP.Modules.Planning.Domain.Enums;

namespace MAPP.Modules.Planning.Application.Plans.Queries.GetPlans;

/// <summary>
/// Plan brief DTO following Ardalis patterns
/// </summary>
public class PlanBriefDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PlanStatus Status { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public string? OwnerId { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public int ItemsCount { get; set; }
    public int CompletedItemsCount { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Plan, PlanBriefDto>()
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.Name))
                .ForMember(d => d.ItemsCount, opt => opt.MapFrom(s => s.Items.Count))
                .ForMember(d => d.CompletedItemsCount, opt => opt.MapFrom(s => s.Items.Count(i => i.Status == PlanItemStatus.Completed)));
        }
    }
}
