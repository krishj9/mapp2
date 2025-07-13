using Ardalis.GuardClauses;
using MAPP.BuildingBlocks.Domain.Common;
using MAPP.Modules.Observations.Domain.Events;
using MAPP.Modules.Observations.Domain.ValueObjects;
using MAPP.Modules.Observations.Domain.Enums;

namespace MAPP.Modules.Observations.Domain.Entities;

/// <summary>
/// Observation aggregate root following DDD patterns
/// </summary>
public class Observation : BaseAuditableEntity
{
    private readonly List<ObservationArtifact> _mediaItems = new();
    private readonly List<int> _progressionPointIds = new();
    private readonly List<Tag> _tags = new();

    public long ChildId { get; private set; }
    public string ChildName { get; private set; } = string.Empty;
    public long TeacherId { get; private set; }
    public string TeacherName { get; private set; } = string.Empty;
    public int DomainId { get; private set; }
    public string DomainName { get; private set; } = string.Empty;
    public int AttributeId { get; private set; }
    public string AttributeName { get; private set; } = string.Empty;
    public string ObservationText { get; private set; } = string.Empty;
    public DateTime ObservationDate { get; private set; }
    public string? LearningContext { get; private set; }
    public bool IsDraft { get; private set; }

    public IReadOnlyCollection<ObservationArtifact> MediaItems => _mediaItems.AsReadOnly();
    public IReadOnlyCollection<int> ProgressionPointIds => _progressionPointIds.AsReadOnly();
    public IReadOnlyCollection<Tag> Tags => _tags.AsReadOnly();

    // Private constructor for EF Core
    private Observation() { }

    public Observation(
        long childId,
        string childName,
        long teacherId,
        string teacherName,
        int domainId,
        string domainName,
        int attributeId,
        string attributeName,
        string observationText,
        DateTime observationDate,
        string? learningContext = null,
        bool isDraft = false)
    {
        ChildId = Guard.Against.NegativeOrZero(childId, nameof(childId));
        ChildName = Guard.Against.NullOrEmpty(childName, nameof(childName));
        TeacherId = Guard.Against.NegativeOrZero(teacherId, nameof(teacherId));
        TeacherName = Guard.Against.NullOrEmpty(teacherName, nameof(teacherName));
        DomainId = Guard.Against.NegativeOrZero(domainId, nameof(domainId));
        DomainName = Guard.Against.NullOrEmpty(domainName, nameof(domainName));
        AttributeId = Guard.Against.NegativeOrZero(attributeId, nameof(attributeId));
        AttributeName = Guard.Against.NullOrEmpty(attributeName, nameof(attributeName));
        ObservationText = Guard.Against.NullOrEmpty(observationText, nameof(observationText));
        ObservationDate = observationDate;
        LearningContext = learningContext;
        IsDraft = isDraft;

        AddDomainEvent(new ObservationCreatedEvent(this));
    }

    public void UpdateObservation(string observationText, DateTime? observationDate = null, string? learningContext = null)
    {
        ObservationText = Guard.Against.NullOrEmpty(observationText, nameof(observationText));
        
        if (observationDate.HasValue)
        {
            ObservationDate = observationDate.Value;
        }
        
        LearningContext = learningContext;
        
        AddDomainEvent(new ObservationUpdatedEvent(this));
    }

    public void UpdateClassification(int domainId, string domainName, int attributeId, string attributeName, List<int> progressionPointIds)
    {
        DomainId = Guard.Against.NegativeOrZero(domainId, nameof(domainId));
        DomainName = Guard.Against.NullOrEmpty(domainName, nameof(domainName));
        AttributeId = Guard.Against.NegativeOrZero(attributeId, nameof(attributeId));
        AttributeName = Guard.Against.NullOrEmpty(attributeName, nameof(attributeName));
        
        _progressionPointIds.Clear();
        _progressionPointIds.AddRange(progressionPointIds);
        
        AddDomainEvent(new ObservationUpdatedEvent(this));
    }

    public void SetDraftStatus(bool isDraft)
    {
        IsDraft = isDraft;
        
        if (!isDraft)
        {
            AddDomainEvent(new ObservationPublishedEvent(this));
        }
    }

    public ObservationArtifact AddMediaItem(
        string originalFileName,
        string contentType,
        long fileSizeBytes,
        string? caption = null,
        int displayOrder = 0,
        string? metadata = null)
    {
        var mediaItem = new ObservationArtifact(
            Id,
            originalFileName,
            contentType,
            fileSizeBytes,
            caption,
            displayOrder,
            metadata);

        _mediaItems.Add(mediaItem);
        return mediaItem;
    }

    public void RemoveMediaItem(ObservationArtifact mediaItem)
    {
        _mediaItems.Remove(mediaItem);
    }

    public void AddTag(string tagValue)
    {
        var tag = new Tag(tagValue);
        if (!_tags.Contains(tag))
        {
            _tags.Add(tag);
        }
    }

    public void RemoveTag(string tagValue)
    {
        var tag = new Tag(tagValue);
        _tags.Remove(tag);
    }

    public void ClearTags()
    {
        _tags.Clear();
    }

    public void SetTags(List<string> tags)
    {
        _tags.Clear();
        foreach (var tagValue in tags)
        {
            AddTag(tagValue);
        }
    }

    public void AddProgressionPoint(int progressionPointId)
    {
        if (!_progressionPointIds.Contains(progressionPointId))
        {
            _progressionPointIds.Add(progressionPointId);
        }
    }

    public void RemoveProgressionPoint(int progressionPointId)
    {
        _progressionPointIds.Remove(progressionPointId);
    }

    public void SetProgressionPoints(List<int> progressionPointIds)
    {
        _progressionPointIds.Clear();
        _progressionPointIds.AddRange(progressionPointIds);
    }
} 