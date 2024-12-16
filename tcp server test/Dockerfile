# Base runtime image for production (Alpine-based for minimal memory usage)
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app
# Expose the application port (match your server port)
EXPOSE 5000

# Build stage using the lightweight Alpine SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies (optimize caching)
COPY ["tcp server test/Server.csproj", "tcp server test/"]
RUN dotnet restore "./tcp server test/Server.csproj"

# Copy the rest of the application files
COPY . .
WORKDIR "/src/tcp server test"

# Build the application in Release mode
RUN dotnet build "./Server.csproj" -c Release -o /app/build

# Publish the application with trimming and ready-to-run optimizations
RUN dotnet publish "./Server.csproj" -c Release -o /app/publish \
    /p:PublishTrimmed=true \
    /p:PublishReadyToRun=true \
    /p:PublishSingleFile=true \
    /p:SelfContained=false

# Final runtime image
FROM base AS final
WORKDIR /app

# Copy the published files
COPY --from=build /app/publish .

# Set environment variables for memory efficiency
ENV DOTNET_GCHeapHardLimit=256MB
ENV DOTNET_TieredPGO=1

# Run the application
ENTRYPOINT ["dotnet", "Server.dll"]
