using MAPP.Modules.Observations.Domain.Entities;
using Xunit;

namespace MAPP.Modules.Observations.Domain.UnitTests.Entities;

public class ProgressionPointTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateProgressionPoint()
    {
        // Arrange
        var points = 1;
        var attributeId = 1;
        var title = "Emerging";
        var description = "Shows emerging skills";
        var order = 1;
        var sortOrder = 1;

        // Act
        var progressionPoint = ProgressionPoint.Create(points, attributeId, title, description, order, sortOrder);

        // Assert
        Assert.Equal(points, progressionPoint.Points);
        Assert.Equal(attributeId, progressionPoint.AttributeId);
        Assert.Equal(title, progressionPoint.Title);
        Assert.Equal(description, progressionPoint.Description);
        Assert.Equal(order.ToString(), progressionPoint.Order);
        Assert.Equal(sortOrder, progressionPoint.SortOrder);
        Assert.True(progressionPoint.IsActive);
    }

    [Theory]
    [InlineData("")]
    public void Create_WithInvalidTitle_ShouldThrowException(string invalidTitle)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ProgressionPoint.Create(1, 1, invalidTitle, "Description", 1, 1));
    }

    [Fact]
    public void Create_WithNullTitle_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ProgressionPoint.Create(1, 1, null!, "Description", 1, 1));
    }

    [Theory]
    [InlineData("")]
    public void Create_WithInvalidDescription_ShouldThrowException(string invalidDescription)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            ProgressionPoint.Create(1, 1, "Title", invalidDescription, 1, 1));
    }

    [Fact]
    public void Create_WithNullDescription_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            ProgressionPoint.Create(1, 1, "Title", null!, 1, 1));
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var progressionPoint = ProgressionPoint.Create(1, 1, "Emerging", "Shows emerging skills", 1, 1);
        var newPoints = 2;
        var newTitle = "Developing";
        var newDescription = "Shows developing skills";
        var newOrder = 2;
        var newCategoryInfo = "Updated category information";
        var newSortOrder = 2;

        // Act
        progressionPoint.UpdateDetails(newPoints, newTitle, newDescription, newOrder, newCategoryInfo, newSortOrder);

        // Assert
        Assert.Equal(newPoints, progressionPoint.Points);
        Assert.Equal(newTitle, progressionPoint.Title);
        Assert.Equal(newDescription, progressionPoint.Description);
        Assert.Equal(newOrder.ToString(), progressionPoint.Order);
        Assert.Equal(newCategoryInfo, progressionPoint.CategoryInformation);
        Assert.Equal(newSortOrder, progressionPoint.SortOrder);
    }

    [Fact]
    public void SetActiveStatus_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var progressionPoint = ProgressionPoint.Create(1, 1, "Emerging", "Shows emerging skills", 1, 1);
        Assert.True(progressionPoint.IsActive); // Initially active

        // Act
        progressionPoint.SetActiveStatus(false);

        // Assert
        Assert.False(progressionPoint.IsActive);

        // Act
        progressionPoint.SetActiveStatus(true);

        // Assert
        Assert.True(progressionPoint.IsActive);
    }
}
