# --- BƯỚC 1: MÔI TRƯỜNG BUILD ---
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy toàn bộ mã nguồn của CẢ DỰ ÁN (API và Services) vào Docker
COPY . .

# Restore và Build từ thư mục gốc
RUN dotnet restore "UrlShortener.API/UrlShortener.API.csproj"
RUN dotnet publish "UrlShortener.API/UrlShortener.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- BƯỚC 2: MÔI TRƯỜNG CHẠY (RUNTIME) ---
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]