# Use the official .NET 9 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Use the .NET 9 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/Services/MAPP.Services.Observations/MAPP.Services.Observations.csproj", "src/Services/MAPP.Services.Observations/"]
COPY ["src/Modules/Observations/MAPP.Modules.Observations.Application/MAPP.Modules.Observations.Application.csproj", "src/Modules/Observations/MAPP.Modules.Observations.Application/"]
COPY ["src/Modules/Observations/MAPP.Modules.Observations.Domain/MAPP.Modules.Observations.Domain.csproj", "src/Modules/Observations/MAPP.Modules.Observations.Domain/"]
COPY ["src/Modules/Observations/MAPP.Modules.Observations.Infrastructure/MAPP.Modules.Observations.Infrastructure.csproj", "src/Modules/Observations/MAPP.Modules.Observations.Infrastructure/"]
COPY ["src/BuildingBlocks/MAPP.BuildingBlocks.Application/MAPP.BuildingBlocks.Application.csproj", "src/BuildingBlocks/MAPP.BuildingBlocks.Application/"]
COPY ["src/BuildingBlocks/MAPP.BuildingBlocks.Domain/MAPP.BuildingBlocks.Domain.csproj", "src/BuildingBlocks/MAPP.BuildingBlocks.Domain/"]
COPY ["src/BuildingBlocks/MAPP.BuildingBlocks.Infrastructure/MAPP.BuildingBlocks.Infrastructure.csproj", "src/BuildingBlocks/MAPP.BuildingBlocks.Infrastructure/"]
COPY ["src/BuildingBlocks/MAPP.BuildingBlocks.Web/MAPP.BuildingBlocks.Web.csproj", "src/BuildingBlocks/MAPP.BuildingBlocks.Web/"]
COPY ["Directory.Build.props", "Directory.Build.props"]
COPY ["Directory.Packages.props", "Directory.Packages.props"]
COPY ["global.json", "global.json"]

RUN dotnet restore "src/Services/MAPP.Services.Observations/MAPP.Services.Observations.csproj"

# Copy source code
COPY . .
WORKDIR "/src/src/Services/MAPP.Services.Observations"
RUN dotnet build "MAPP.Services.Observations.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MAPP.Services.Observations.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "MAPP.Services.Observations.dll"]
