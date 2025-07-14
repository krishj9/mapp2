using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MAPP.Modules.Observations.Application.Classifications.Queries.GetClassificationData;
using MAPP.Modules.Observations.Application.Classifications.Services;
using MAPP.Modules.Observations.Application.Common.Interfaces;
using MAPP.Modules.Observations.Domain.Entities;

namespace MAPP.Modules.Observations.Application.UnitTests.Classifications.Queries;

public class GetClassificationDataQueryHandlerTests
{
    private readonly Mock<IObservationsDbContext> _mockContext;
    private readonly Mock<IClassificationCacheService> _mockCacheService;
    private readonly Mock<ILogger<GetClassificationDataQueryHandler>> _mockLogger;
    private readonly GetClassificationDataQueryHandler _handler;

    public GetClassificationDataQueryHandlerTests()
    {
        _mockContext = new Mock<IObservationsDbContext>();
        _mockCacheService = new Mock<IClassificationCacheService>();
        _mockLogger = new Mock<ILogger<GetClassificationDataQueryHandler>>();
        
        _handler = new GetClassificationDataQueryHandler(
            _mockContext.Object,
            _mockCacheService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_WhenCacheHit_ShouldReturnCachedData()
    {
        // Arrange
        var query = new GetClassificationDataQuery { IncludeInactive = false };
        var cachedData = new ClassificationDataVm
        {
            Domains = new List<DomainDto>
            {
                new DomainDto { Id = 1, Name = "Physical Development" }
            }
        };

        _mockCacheService
            .Setup(x => x.GetCachedClassificationDataAsync(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedData);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Domains);
        Assert.Equal("Physical Development", result.Domains.First().Name);
        
        _mockCacheService.Verify(x => x.GetCachedClassificationDataAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(x => x.ObservationDomains, Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCacheMiss_ShouldQueryDatabaseAndCache()
    {
        // Arrange
        var query = new GetClassificationDataQuery { IncludeInactive = false };
        
        _mockCacheService
            .Setup(x => x.GetCachedClassificationDataAsync(false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClassificationDataVm?)null);

        var domains = CreateTestDomains();
        var mockDbSet = CreateMockDbSet(domains);
        
        _mockContext.Setup(x => x.ObservationDomains).Returns(mockDbSet.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Domains);
        Assert.Equal("Physical Development", result.Domains.First().Name);
        
        _mockCacheService.Verify(x => x.GetCachedClassificationDataAsync(false, It.IsAny<CancellationToken>()), Times.Once);
        _mockCacheService.Verify(x => x.SetCachedClassificationDataAsync(It.IsAny<ClassificationDataVm>(), false, It.IsAny<CancellationToken>()), Times.Once);
    }

    private List<ObservationDomain> CreateTestDomains()
    {
        var domain = ObservationDomain.Create(1, "Physical Development", "Physical", "Physical Development", 1);
        var attribute = ObservationAttribute.Create(1, 1, "Uses large muscles", 1, 1);
        var progressionPoint = ProgressionPoint.Create(1, 1, "Emerging", "Shows emerging skills", 1, 1);
        
        attribute.AddProgressionPoint(progressionPoint);
        domain.AddAttribute(attribute);
        
        return new List<ObservationDomain> { domain };
    }

    private Mock<DbSet<ObservationDomain>> CreateMockDbSet(List<ObservationDomain> data)
    {
        var queryable = data.AsQueryable();
        var mockDbSet = new Mock<DbSet<ObservationDomain>>();
        
        mockDbSet.As<IQueryable<ObservationDomain>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockDbSet.As<IQueryable<ObservationDomain>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockDbSet.As<IQueryable<ObservationDomain>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockDbSet.As<IQueryable<ObservationDomain>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
        
        return mockDbSet;
    }
}
