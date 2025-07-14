namespace MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;

/// <summary>
/// View model for classification data matching the JSON structure
/// </summary>
public class ClassificationDataVm
{
    public List<DomainDto> Domains { get; set; } = new();
}

public class DomainDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryTitle { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<AttributeDto> Attributes { get; set; } = new();
}

public class AttributeDto
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
    public List<ProgressionPointDto> ProgressionPoints { get; set; } = new();
}

public class ProgressionPointDto
{
    public int ProgressionId { get; set; }
    public int Points { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Order { get; set; }
    public string? CategoryInformation { get; set; }
    public int SortOrder { get; set; }
}
