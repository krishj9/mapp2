using AutoMapper;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Application.Observations.Queries.GetObservations;

/// <summary>
/// Observation brief DTO following Ardalis patterns
/// </summary>
public class ObservationBriefDto
{
    public int Id { get; set; }
    public long ChildId { get; set; }
    public string ChildName { get; set; } = string.Empty;
    public long TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string DomainName { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public string ObservationTextPreview { get; set; } = string.Empty;
    public DateTime ObservationDate { get; set; }
    public string? LearningContext { get; set; }
    public bool IsDraft { get; set; }
    public List<string> Tags { get; set; } = new();
    public int MediaItemCount { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Observation, ObservationBriefDto>()
                .ForMember(d => d.ObservationTextPreview, opt => opt.MapFrom(s => s.ObservationText.Length > 100 ? s.ObservationText.Substring(0, 100) + "..." : s.ObservationText))
                .ForMember(d => d.Tags, opt => opt.MapFrom(s => s.Tags.Select(t => t.Value).ToList()))
                .ForMember(d => d.MediaItemCount, opt => opt.MapFrom(s => s.MediaItems.Count));
        }
    }
}
