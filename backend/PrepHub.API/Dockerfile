# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PrepHub.sln ./
COPY PrepHub.API/PrepHub.API.csproj PrepHub.API/
COPY PrepHub.Core/PrepHub.Core.csproj PrepHub.Core/
COPY PrepHub.Infrastructure/PrepHub.Infrastructure.csproj PrepHub.Infrastructure/

# Restore packages
RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish PrepHub.API/PrepHub.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "PrepHub.API.dll"]
