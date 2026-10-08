# Build stage: restore and publish with the full SDK image.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HelloApi/HelloApi.csproj src/HelloApi/
RUN dotnet restore src/HelloApi/HelloApi.csproj

COPY src/ src/
RUN dotnet publish src/HelloApi/HelloApi.csproj -c Release -o /app --no-restore

# Runtime stage: a small "chiseled" image with no shell, running as a non-root user.
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled
WORKDIR /app
COPY --from=build /app .

# The .NET 8+ images listen on port 8080 and run as the "app" user by default.
EXPOSE 8080
ENTRYPOINT ["dotnet", "HelloApi.dll"]
