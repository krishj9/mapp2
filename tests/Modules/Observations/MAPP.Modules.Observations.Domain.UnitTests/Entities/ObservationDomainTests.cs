using MAPP.Modules.Observations.Domain.Entities;
using Xunit;

namespace MAPP.Modules.Observations.Domain.UnitTests.Entities;

public class ObservationDomainTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateDomain()
    {
        // Arrange
        var id = 1;
        var name = "Physical Development";
        var categoryName = "Physical";
        var categoryTitle = "Physical Development";
        var sortOrder = 1;

        // Act
        var domain = ObservationDomain.Create(id, name, categoryName, categoryTitle, sortOrder);

        // Assert
        Assert.Equal(id, domain.Id);
        Assert.Equal(name, domain.Name);
        Assert.Equal(categoryName, domain.CategoryName);
        Assert.Equal(categoryTitle, domain.CategoryTitle);
        Assert.Equal(sortOrder, domain.SortOrder);
        Assert.True(domain.IsActive);
        Assert.Empty(domain.Attributes);
    }

    [Theory]
    [InlineData("")]
    public void Create_WithInvalidName_ShouldThrowException(string invalidName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            ObservationDomain.Create(1, invalidName, "Physical", "Physical Development", 1));
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            ObservationDomain.Create(1, null!, "Physical", "Physical Development", 1));
    }

    [Fact]
    public void AddAttribute_WithValidAttribute_ShouldAddToCollection()
    {
        // Arrange
        var domain = ObservationDomain.Create(1, "Physical Development", "Physical", "Physical Development", 1);
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);

        // Act
        domain.AddAttribute(attribute);

        // Assert
        Assert.Single(domain.Attributes);
        Assert.Contains(attribute, domain.Attributes);
    }

    [Fact]
    public void AddAttribute_WithDuplicateId_ShouldNotAddDuplicate()
    {
        // Arrange
        var domain = ObservationDomain.Create(1, "Physical Development", "Physical", "Physical Development", 1);
        var attribute1 = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        var attribute2 = ObservationAttribute.Create(1, 2, "Different attribute", 2, 1);

        // Act
        domain.AddAttribute(attribute1);
        domain.AddAttribute(attribute2); // Same ID, should not be added

        // Assert
        Assert.Single(domain.Attributes);
        Assert.Contains(attribute1, domain.Attributes);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldUpdateProperties()
    {
        // Arrange
        var domain = ObservationDomain.Create(1, "Physical Development", "Physical", "Physical Development", 1);
        var newName = "Updated Physical Development";
        var newCategoryName = "Updated Physical";
        var newCategoryTitle = "Updated Physical Development";
        var newSortOrder = 2;

        // Act
        domain.UpdateDetails(newName, newCategoryName, newCategoryTitle, newSortOrder);

        // Assert
        Assert.Equal(newName, domain.Name);
        Assert.Equal(newCategoryName, domain.CategoryName);
        Assert.Equal(newCategoryTitle, domain.CategoryTitle);
        Assert.Equal(newSortOrder, domain.SortOrder);
    }

    [Fact]
    public void SetActiveStatus_ShouldUpdateIsActiveProperty()
    {
        // Arrange
        var domain = ObservationDomain.Create(1, "Physical Development", "Physical", "Physical Development", 1);
        Assert.True(domain.IsActive); // Initially active

        // Act
        domain.SetActiveStatus(false);

        // Assert
        Assert.False(domain.IsActive);

        // Act
        domain.SetActiveStatus(true);

        // Assert
        Assert.True(domain.IsActive);
    }
}
