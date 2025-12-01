FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV DOTNET_ENVIRONMENT=Production
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENV TZ=Europe/Istanbul

# Create non-root user
RUN useradd -m appuser

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BaseProject.WebAPI/BaseProject.WebAPI.csproj", "BaseProject.WebAPI/"]
COPY ["BaseProject.Application/BaseProject.Application.csproj", "BaseProject.Application/"]
COPY ["BaseProject.Domain/BaseProject.Domain.csproj", "BaseProject.Domain/"]
COPY ["BaseProject.Infrastructure.Contract/BaseProject.Infrastructure.Contract.csproj", "BaseProject.Infrastructure.Contract/"]
COPY ["BaseProject.Infrastructure/BaseProject.Infrastructure.csproj", "BaseProject.Infrastructure/"]

RUN dotnet restore "./BaseProject.WebAPI/BaseProject.WebAPI.csproj"

COPY . .
WORKDIR "/src/BaseProject.WebAPI"
RUN dotnet build "./BaseProject.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
RUN dotnet publish "./OtoSicil.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Healthcheck için curl ekle
USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Set proper permissions and switch to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Healthcheck
HEALTHCHECK --interval=5s --timeout=3s --start-period=5s --retries=5 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "BaseProject.WebAPI.dll"]