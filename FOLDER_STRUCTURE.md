# MAPP Monorepo Structure
## Adapted from Ardalis Clean Architecture with Multi-Domain Support

```
MAPPV2/
├── README.md
├── .gitignore
├── global.json                          # .NET SDK version pinning
├── Directory.Build.props                # Global MSBuild properties
├── Directory.Packages.props             # Central package management
├── MAPP.sln                            # Main solution file
├── 
├── # === ASPIRE ORCHESTRATION (Flexible) ===
├── orchestration/
│   ├── MAPP.AppHost/                   # Full solution orchestration
│   │   ├── MAPP.AppHost.csproj
│   │   ├── Program.cs                  # Orchestrates all domains + GenAI + Web
│   │   ├── appsettings.json
│   │   └── Properties/
│   ├── MAPP.Planning.AppHost/          # Planning domain only orchestration
│   │   ├── MAPP.Planning.AppHost.csproj
│   │   ├── Program.cs                  # Planning API + Planning GenAI + Planning MFE
│   │   └── appsettings.json
│   ├── MAPP.Observations.AppHost/      # Observations domain only
│   ├── MAPP.UserManagement.AppHost/    # UserManagement domain only
│   ├── MAPP.Reports.AppHost/           # Reports domain only
│   └── MAPP.ServiceDefaults/           # Shared Aspire service defaults
│       ├── MAPP.ServiceDefaults.csproj
│       └── Extensions.cs
├── 
├── # === BUILDING BLOCKS (Shared Kernel) ===
├── src/
│   ├── BuildingBlocks/                 # Based on Ardalis patterns
│   │   ├── MAPP.BuildingBlocks.Domain/
│   │   │   ├── Common/
│   │   │   │   ├── BaseEntity.cs       # From Ardalis
│   │   │   │   ├── BaseAuditableEntity.cs
│   │   │   │   ├── ValueObject.cs
│   │   │   │   └── IAggregateRoot.cs
│   │   │   ├── Events/
│   │   │   │   ├── BaseEvent.cs        # From Ardalis
│   │   │   │   └── IEventHandler.cs
│   │   │   ├── Interfaces/
│   │   │   │   ├── IRepository.cs
│   │   │   │   ├── IUnitOfWork.cs
│   │   │   │   └── IDomainEventService.cs
│   │   │   └── Specifications/
│   │   │       ├── BaseSpecification.cs
│   │   │       └── ISpecification.cs
│   │   ├── MAPP.BuildingBlocks.Application/
│   │   │   ├── Common/
│   │   │   │   ├── Interfaces/
│   │   │   │   │   ├── IApplicationDbContext.cs
│   │   │   │   │   ├── IDateTime.cs
│   │   │   │   │   ├── ICurrentUserService.cs
│   │   │   │   │   └── IIdentityService.cs
│   │   │   │   ├── Behaviours/          # From Ardalis
│   │   │   │   │   ├── AuthorizationBehaviour.cs
│   │   │   │   │   ├── ValidationBehaviour.cs
│   │   │   │   │   ├── PerformanceBehaviour.cs
│   │   │   │   │   └── UnhandledExceptionBehaviour.cs
│   │   │   │   ├── Exceptions/
│   │   │   │   │   ├── ValidationException.cs
│   │   │   │   │   ├── NotFoundException.cs
│   │   │   │   │   └── ForbiddenAccessException.cs
│   │   │   │   └── Models/
│   │   │   │       ├── Result.cs       # From Ardalis
│   │   │   │       ├── PaginatedList.cs
│   │   │   │       └── LookupDto.cs
│   │   │   ├── Mappings/
│   │   │   │   └── IMapFrom.cs
│   │   │   └── Security/
│   │   │       └── IUser.cs
│   │   ├── MAPP.BuildingBlocks.Infrastructure/
│   │   │   ├── Data/
│   │   │   │   ├── BaseDbContext.cs
│   │   │   │   ├── ApplicationDbContextInitialiser.cs
│   │   │   │   ├── Configurations/
│   │   │   │   └── Interceptors/
│   │   │   │       └── AuditableEntitySaveChangesInterceptor.cs
│   │   │   ├── Identity/
│   │   │   │   ├── IdentityService.cs
│   │   │   │   └── IdentityResultExtensions.cs
│   │   │   ├── Services/
│   │   │   │   ├── DateTimeService.cs
│   │   │   │   └── DomainEventService.cs
│   │   │   └── DependencyInjection.cs
│   │   └── MAPP.BuildingBlocks.Web/    # FastEndpoints patterns
│   │       ├── Endpoints/
│   │       │   ├── BaseEndpoint.cs     # FastEndpoints base
│   │       │   └── EndpointExtensions.cs
│   │       ├── Filters/
│   │       │   ├── ApiExceptionFilterAttribute.cs
│   │       │   └── ValidationFilter.cs
│   │       ├── Services/
│   │       │   └── CurrentUserService.cs
│   │       └── DependencyInjection.cs
│   │
│   ├── # === DOMAIN MODULES (Bounded Contexts) ===
│   ├── Modules/
│   │   ├── Planning/                   # Planning Bounded Context
│   │   │   ├── MAPP.Modules.Planning.Domain/
│   │   │   │   ├── Common/
│   │   │   │   ├── Entities/
│   │   │   │   │   ├── Plan.cs
│   │   │   │   │   └── PlanItem.cs
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   ├── PlanId.cs
│   │   │   │   │   └── Priority.cs
│   │   │   │   ├── Events/
│   │   │   │   │   ├── PlanCreatedEvent.cs
│   │   │   │   │   └── PlanCompletedEvent.cs
│   │   │   │   ├── Enums/
│   │   │   │   │   └── PlanStatus.cs
│   │   │   │   └── Constants/
│   │   │   │       └── Roles.cs
│   │   │   ├── MAPP.Modules.Planning.Application/
│   │   │   │   ├── Common/
│   │   │   │   │   └── Interfaces/
│   │   │   │   │       └── IPlanningDbContext.cs
│   │   │   │   ├── Plans/
│   │   │   │   │   ├── Commands/
│   │   │   │   │   │   ├── CreatePlan/
│   │   │   │   │   │   │   ├── CreatePlanCommand.cs
│   │   │   │   │   │   │   ├── CreatePlanCommandHandler.cs
│   │   │   │   │   │   │   └── CreatePlanCommandValidator.cs
│   │   │   │   │   │   ├── UpdatePlan/
│   │   │   │   │   │   └── DeletePlan/
│   │   │   │   │   ├── Queries/
│   │   │   │   │   │   ├── GetPlans/
│   │   │   │   │   │   │   ├── GetPlansQuery.cs
│   │   │   │   │   │   │   ├── GetPlansQueryHandler.cs
│   │   │   │   │   │   │   ├── PlanBriefDto.cs
│   │   │   │   │   │   │   └── PlansVm.cs
│   │   │   │   │   │   └── GetPlanDetail/
│   │   │   │   │   └── EventHandlers/
│   │   │   │   │       └── PlanCreatedEventHandler.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   ├── MAPP.Modules.Planning.Infrastructure/
│   │   │   │   ├── Data/
│   │   │   │   │   ├── PlanningDbContext.cs
│   │   │   │   │   ├── Configurations/
│   │   │   │   │   │   ├── PlanConfiguration.cs
│   │   │   │   │   │   └── PlanItemConfiguration.cs
│   │   │   │   │   └── Migrations/
│   │   │   │   ├── Services/
│   │   │   │   │   └── PlanningAiService.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   └── MAPP.Modules.Planning.Tests/
│   │   │       ├── Application/
│   │   │       │   ├── Common/
│   │   │       │   └── Plans/
│   │   │       │       ├── Commands/
│   │   │       │       └── Queries/
│   │   │       └── Domain/
│   │   │           └── Entities/
│   │   ├── Observations/               # Similar structure
│   │   ├── UserManagement/             # Similar structure  
│   │   └── Reports/                    # Similar structure
│   │
│   ├── # === API SERVICES (FastEndpoints Hosts) ===
│   ├── Services/
│   │   ├── MAPP.Services.Planning/     # Planning API Host
│   │   │   ├── MAPP.Services.Planning.csproj
│   │   │   ├── Program.cs              # FastEndpoints + Planning module
│   │   │   ├── Endpoints/
│   │   │   │   └── Plans/
│   │   │   │       ├── Create.cs       # FastEndpoints pattern
│   │   │   │       ├── GetAll.cs
│   │   │   │       ├── GetById.cs
│   │   │   │       ├── Update.cs
│   │   │   │       └── Delete.cs
│   │   │   ├── appsettings.json
│   │   │   └── Properties/
│   │   ├── MAPP.Services.Observations/
│   │   ├── MAPP.Services.UserManagement/
│   │   ├── MAPP.Services.Reports/
│   │   └── genai/                      # Python FastAPI services
│   │       ├── planning-ai/
│   │       ├── observations-ai/
│   │       ├── reports-ai/
│   │       └── shared/
│   │
│   └── # === FRONTEND ===
│   └── Web/
│       ├── shell/                      # Main shell application
│       ├── planning-mfe/               # Planning microfrontend
│       ├── observations-mfe/
│       ├── usermanagement-mfe/
│       ├── reports-mfe/
│       └── shared/
├── 
├── # === TESTING ===
├── tests/
│   ├── Application.IntegrationTests/
│   ├── Application.UnitTests/
│   └── Domain.UnitTests/
├── 
├── # === DEPLOYMENT & INFRASTRUCTURE ===
├── deployment/
├── pipelines/
├── docs/
└── tools/
```

## Key Orchestration Flexibility Features:

### 1. **Multiple AppHost Options:**
- `MAPP.AppHost` - Full solution (all domains + GenAI + Web)
- `MAPP.Planning.AppHost` - Planning domain only
- `MAPP.Observations.AppHost` - Observations domain only
- etc.

### 2. **Shared ServiceDefaults:**
- Common Aspire configurations
- Shared logging, monitoring, health checks
- Database connection patterns

### 3. **Independent API Services:**
- Each domain has its own API host
- Can be run independently or together
- FastEndpoints-based following clean architecture patterns
