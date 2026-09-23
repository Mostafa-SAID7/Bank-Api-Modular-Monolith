# Multi-stage build for Bank API
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first for optimal layer caching
COPY src/Bank.Host/Bank.Host.csproj Bank.Host/
COPY src/Bank.Application/Bank.Application.csproj Bank.Application/
COPY src/Bank.Domain/Bank.Domain.csproj Bank.Domain/
COPY src/Bank.Infrastructure/Bank.Infrastructure.csproj Bank.Infrastructure/
COPY src/BuildingBlocks/Bank.BuildingBlocks.Domain/Bank.BuildingBlocks.Domain.csproj BuildingBlocks/Bank.BuildingBlocks.Domain/
COPY src/BuildingBlocks/Bank.BuildingBlocks.Application/Bank.BuildingBlocks.Application.csproj BuildingBlocks/Bank.BuildingBlocks.Application/
COPY src/BuildingBlocks/Bank.BuildingBlocks.Infrastructure/Bank.BuildingBlocks.Infrastructure.csproj BuildingBlocks/Bank.BuildingBlocks.Infrastructure/
COPY src/Bank.Contracts/Bank.Contracts.csproj Bank.Contracts/
COPY src/Modules/Payments/Bank.Payments.Domain/Bank.Payments.Domain.csproj Modules/Payments/Bank.Payments.Domain/
COPY src/Modules/Payments/Bank.Payments.Application/Bank.Payments.Application.csproj Modules/Payments/Bank.Payments.Application/
COPY src/Modules/Payments/Bank.Payments.Infrastructure/Bank.Payments.Infrastructure.csproj Modules/Payments/Bank.Payments.Infrastructure/
COPY src/Modules/Payments/Bank.Payments.Presentation/Bank.Payments.Presentation.csproj Modules/Payments/Bank.Payments.Presentation/
COPY src/Modules/Notifications/Bank.Notifications.Domain/Bank.Notifications.Domain.csproj Modules/Notifications/Bank.Notifications.Domain/
COPY src/Modules/Notifications/Bank.Notifications.Application/Bank.Notifications.Application.csproj Modules/Notifications/Bank.Notifications.Application/
COPY src/Modules/Notifications/Bank.Notifications.Infrastructure/Bank.Notifications.Infrastructure.csproj Modules/Notifications/Bank.Notifications.Infrastructure/
COPY src/Modules/Notifications/Bank.Notifications.Presentation/Bank.Notifications.Presentation.csproj Modules/Notifications/Bank.Notifications.Presentation/
COPY NuGet.Config .

# Restore dependencies
RUN dotnet restore Bank.Host/Bank.Host.csproj

# Copy all source code
COPY src/ .

# Stage 2: Publish
WORKDIR /src/Bank.Host
RUN dotnet publish Bank.Host.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime (minimal image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for health checks only
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy published application
COPY --from=build /app/publish .

# Expose API port
EXPOSE 5000

# Environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:5000/health || exit 1

# Run application
ENTRYPOINT ["dotnet", "Bank.Host.dll"]
