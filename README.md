# MAPP - Business Application Monorepo

A modern business application built with Clean Architecture, DDD principles, and microservices patterns.

## Architecture Overview

- **Backend**: .NET 9 with Clean Architecture and DDD
- **APIs**: FastEndpoints for high-performance APIs
- **Frontend**: Angular 20 Micro-frontends (MFEs)
- **AI Services**: Python FastAPI for Gen-AI features
- **Database**: PostgreSQL on GCP CloudSQL
- **Orchestration**: .NET Aspire for local development
- **Deployment**: Google Cloud Run with Azure DevOps CI/CD

## Domain Structure

The application is organized around 4 main business domains:

1. **Planning** - Project and task planning functionality
2. **Observations** - Data collection and monitoring
3. **UserManagement** - User administration and authentication
4. **Reports** - Business intelligence and reporting

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Node.js (latest LTS)
- Docker Desktop
- PostgreSQL (for production) or use in-memory for development

### Running the Application

#### Full Solution (All Domains)
```bash
cd orchestration/MAPP.AppHost
dotnet run
```

#### Individual Domain Development
```bash
# Planning domain only
cd orchestration/MAPP.Planning.AppHost
dotnet run

# Observations domain only
cd orchestration/MAPP.Observations.AppHost
dotnet run
```

#### Individual API Services
```bash
# Planning API
cd src/Services/MAPP.Services.Planning
dotnet run

# Observations API
cd src/Services/MAPP.Services.Observations
dotnet run
```

## Project Structure

```
MAPPV2/
├── orchestration/          # .NET Aspire orchestration
│   ├── MAPP.AppHost/       # Full solution orchestration
│   ├── MAPP.Planning.AppHost/    # Planning domain only
│   └── MAPP.ServiceDefaults/     # Shared Aspire configs
├── src/
│   ├── BuildingBlocks/     # Shared kernel (DDD patterns)
│   ├── Modules/           # Domain modules (bounded contexts)
│   ├── Services/          # API host services
│   └── Web/              # Frontend applications
├── tests/                 # Test projects
└── deployment/           # Infrastructure and deployment
```

## Development Guidelines

### Adding New Features

1. **Domain Logic**: Add to appropriate module in `src/Modules/`
2. **API Endpoints**: Add FastEndpoints to corresponding service in `src/Services/`
3. **Frontend**: Add to appropriate MFE in `src/Web/`

### Testing

```bash
# Run all tests
dotnet test

# Run specific domain tests
dotnet test src/Modules/Planning/MAPP.Modules.Planning.Tests/
```

### Database Migrations

```bash
# Add migration for Planning domain
dotnet ef migrations add "MigrationName" \
  --project src/Modules/Planning/MAPP.Modules.Planning.Infrastructure \
  --startup-project src/Services/MAPP.Services.Planning \
  --output-dir Data/Migrations
```

## Technology Stack

### Backend (.NET 9)
- **FastEndpoints** - High-performance API endpoints
- **MediatR** - CQRS and mediator pattern
- **Entity Framework Core** - Data access
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging

### Frontend (Angular 20)
- **Module Federation** - Micro-frontend architecture
- **Angular Material** - UI components
- **RxJS** - Reactive programming

### AI Services (Python)
- **FastAPI** - High-performance Python APIs
- **Pydantic** - Data validation
- **SQLAlchemy** - Database ORM

### Infrastructure
- **Google Cloud Run** - Container hosting
- **GCP CloudSQL** - PostgreSQL database
- **Azure DevOps** - CI/CD pipelines
- **.NET Aspire** - Local development orchestration

## 🚢 Production Deployment

### Independent Domain Deployment
```bash
# Deploy single domain
./scripts/deploy-domain.sh planning staging

# Deploy all domains
./scripts/deploy-domain.sh all production
```

### CI/CD Pipelines
Each domain has its own Azure DevOps pipeline:
- `.azure/pipelines/planning-domain-pipeline.yml`
- `.azure/pipelines/observations-domain-pipeline.yml`
- `.azure/pipelines/usermanagement-domain-pipeline.yml`
- `.azure/pipelines/reports-domain-pipeline.yml`

### Environment Configuration
Environment-specific settings in each service:
- `appsettings.Development.json`
- `appsettings.Staging.json`
- `appsettings.Production.json`

## 🏥 Health Checks & Monitoring

### Health Check Endpoints
Each service exposes comprehensive health checks:
- `/health` - Detailed health information
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

### Health Check Script
```bash
# Check all domains
./scripts/health-check.sh

# Check specific domain
./scripts/health-check.sh planning development
```

### Monitoring Stack
```bash
cd docker/monitoring
docker-compose -f docker-compose.monitoring.yml up -d
```

Access monitoring:
- **Grafana**: http://localhost:3000
- **Prometheus**: http://localhost:9090
- **Health Dashboard**: http://localhost:8080

## Contributing

1. Follow Clean Architecture principles
2. Use DDD patterns for domain logic
3. Write tests for new features
4. Follow the established folder structure
5. Use FastEndpoints for new API endpoints

## License

Copyright © MAPP 2025
