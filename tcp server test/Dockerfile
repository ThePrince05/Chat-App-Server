# Base runtime image for production (Alpine-based for minimal memory usage)
FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine AS base
WORKDIR /app
EXPOSE 5000

# Build stage using lightweight Alpine SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["tcp server test/Server.csproj", "tcp server test/"]
RUN dotnet restore "./tcp server test/Server.csproj"

# Copy the rest of the application files
COPY . .
WORKDIR "/src/tcp server test"

# Build the application in Release mode
RUN dotnet build "./Server.csproj" -c Release -o /app/build

# Publish the application (self-contained and trimmed for size)
RUN dotnet publish "./Server.csproj" -c Release -o /app/publish \
    /p:PublishTrimmed=true \
    /p:PublishReadyToRun=true \
    /p:PublishSingleFile=true \
    /p:SelfContained=true \
    /p:RuntimeIdentifier=linux-musl-x64

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./Server"]
