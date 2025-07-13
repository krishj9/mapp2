using AutoMapper;
using MAPP.Modules.Observations.Domain.Entities;
using MAPP.Modules.Observations.Domain.Enums;

namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Observation brief DTO following Ardalis patterns
/// </summary>
public class ObservationBriefDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ObservationStatus Status { get; set; }
    public string Priority { get; set; } = string.Empty;
    public DateTimeOffset? ObservedAt { get; set; }
    public string? ObserverId { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public int DataCount { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Observation, ObservationBriefDto>()
                .ForMember(d => d.Priority, opt => opt.MapFrom(s => s.Priority.Name))
                .ForMember(d => d.DataCount, opt => opt.MapFrom(s => s.Data.Count));
        }
    }
}
