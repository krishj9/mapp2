using MAPP.Modules.Observations.Domain.Entities;
using Xunit;

namespace MAPP.Modules.Observations.Domain.UnitTests.Entities;

public class ObservationAttributeTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateAttribute()
    {
        // Arrange
        var id = 1;
        var number = 1;
        var name = "Uses large muscles for movement";
        var domainId = 1;
        var sortOrder = 1;

        // Act
        var attribute = ObservationAttribute.Create(id, number, name, domainId, sortOrder);

        // Assert
        Assert.Equal(id, attribute.Id);
        Assert.Equal(number, attribute.Number);
        Assert.Equal(name, attribute.Name);
        Assert.Equal(domainId, attribute.DomainId);
        Assert.Equal(sortOrder, attribute.SortOrder);
        Assert.True(attribute.IsActive);
        Assert.Empty(attribute.ProgressionPoints);
    }

    [Theory]
    [InlineData("")]
    public void Create_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ObservationAttribute.Create(1, 1, invalidName, 1, 1));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ObservationAttribute.Create(1, 1, null!, 1, 1));
    }

    [Fact]
    public void AddProgressionPoint_WithValidPoint_ShouldAddToCollection()
    {
        // Arrange
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        var progressionPoint = ProgressionPoint.Create(1, 1, "Emerging", "Shows emerging skills", 1, 1);

        // Act
        attribute.AddProgressionPoint(progressionPoint);

        // Assert
        Assert.Single(attribute.ProgressionPoints);
        Assert.Contains(progressionPoint, attribute.ProgressionPoints);
    }

    [Fact]
    public void AddProgressionPoint_WithDuplicateId_ShouldNotAddDuplicate()
    {
        // Arrange
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        var progressionPoint1 = ProgressionPoint.Create(1, 1, "Emerging", "Shows emerging skills", 1, 1);
        var progressionPoint2 = ProgressionPoint.Create(1, 2, "Different point", "Different description", 2, 2);

        // Act
        attribute.AddProgressionPoint(progressionPoint1);
        attribute.AddProgressionPoint(progressionPoint2); // Same ID, should not be added

        // Assert
        Assert.Single(attribute.ProgressionPoints);
        Assert.Contains(progressionPoint1, attribute.ProgressionPoints);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        var newNumber = 2;
        var newName = "Updated attribute name";
        var newCategoryInfo = "Updated category information";
        var newSortOrder = 2;

        // Act
        attribute.UpdateDetails(newNumber, newName, newCategoryInfo, newSortOrder);

        // Assert
        Assert.Equal(newNumber, attribute.Number);
        Assert.Equal(newName, attribute.Name);
        Assert.Equal(newCategoryInfo, attribute.CategoryInformation);
        Assert.Equal(newSortOrder, attribute.SortOrder);
    }

    [Fact]
    public void SetActiveStatus_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        Assert.True(attribute.IsActive); // Initially active

        // Act
        attribute.SetActiveStatus(false);

        // Assert
        Assert.False(attribute.IsActive);

        // Act
        attribute.SetActiveStatus(true);

        // Assert
        Assert.True(attribute.IsActive);
    }
}
