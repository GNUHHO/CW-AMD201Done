# Use the heavy SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project configuration files (.csproj) first to optimize library loading.
COPY ["UrlShortener.API/UrlShortener.API.csproj", "UrlShortener.API/"]
COPY ["UrlShortener.Services/UrlShortener.Services.csproj", "UrlShortener.Services/"]
COPY ["UrlShortener.Data/UrlShortener.Data.csproj", "UrlShortener.Data/"]
COPY ["UrlShortener.Common/UrlShortener.Common.csproj", "UrlShortener.Common/"]

# Run the command to restore (load) the libraries.
RUN dotnet restore "UrlShortener.API/UrlShortener.API.csproj"

# Copy the rest of the source code into it.
COPY . .

# Build project
WORKDIR "/src/UrlShortener.API"
RUN dotnet build "UrlShortener.API.csproj" -c Release -o /app/build

# Publishing
FROM build AS publish
RUN dotnet publish "UrlShortener.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the lightweight Runtime engine to run your application (Production Environment).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy only the packaged file portion from Phase 2.
COPY --from=publish /app/publish .

# Command to launch the application
ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]