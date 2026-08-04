# ============================================
# Stage 1: Build
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY Whisper.sln .
COPY Whisper.Api/Whisper.Api.csproj Whisper.Api/
COPY Whisper.Application/Whisper.Application.csproj Whisper.Application/
COPY Whisper.Domain/Whisper.Domain.csproj Whisper.Domain/
COPY Whisper.Infrastructure/Whisper.Infrastructure.csproj Whisper.Infrastructure/
COPY Whisper.Tests/Whisper.Tests.csproj Whisper.Tests/

# Restore dependencies (cached unless .csproj files change)
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish Whisper.Api/Whisper.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ============================================
# Stage 2: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser -s /bin/false appuser

COPY --from=build /app/publish .

# Default port configuration (Render uses PORT env var)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Run as non-root user
USER appuser

ENTRYPOINT ["dotnet", "Whisper.Api.dll"]
