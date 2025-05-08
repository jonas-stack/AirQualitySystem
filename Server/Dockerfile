# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . ./
RUN ls -la ./Startup/
RUN dotnet restore "./Startup/Startup.csproj"
RUN dotnet publish "./Startup/Startup.csproj" -c Release -o /app
RUN ls -la /app/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
RUN ls -la /app/
ENTRYPOINT ["dotnet", "Startup.dll"]