# BUILD ENVIRONMENT
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire project's source code (APIs and services) into Docker.
COPY . .

# Restore and Build from the root directory
RUN dotnet restore "UrlShortener.API/UrlShortener.API.csproj"
RUN dotnet publish "UrlShortener.API/UrlShortener.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

#RUNNING ENVIRONMENT (RUNTIME) 
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]