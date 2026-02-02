# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY TalepYonetimi.csproj ./
RUN dotnet restore TalepYonetimi.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish TalepYonetimi.csproj -c Release -o /app/publish

# Copy appsettings explicitly to publish folder
COPY appsettings.Production.json /app/publish/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Install timezone data
RUN apt-get update && apt-get install -y tzdata && rm -rf /var/lib/apt/lists/*

# Set timezone to Turkey
ENV TZ=Europe/Istanbul

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "TalepYonetimi.dll"]
