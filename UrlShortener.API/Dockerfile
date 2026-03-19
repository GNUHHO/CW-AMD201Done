# --- BƯỚC 1: MÔI TRƯỜNG BUILD ---
# Dùng bộ SDK đầy đủ của .NET 10 để biên dịch code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy file cấu hình và tải các thư viện cần thiết
COPY ["UrlShortener.API.csproj", "./"]
RUN dotnet restore "UrlShortener.API.csproj"

# Copy toàn bộ code còn lại vào và tiến hành Build (Đóng gói)
COPY . .
RUN dotnet publish "UrlShortener.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- BƯỚC 2: MÔI TRƯỜNG CHẠY (RUNTIME) ---
# Chỉ dùng bản ASP.NET siêu nhẹ để chạy, giúp file gửi đi cực kỳ nhỏ
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Mở cửa số 8080 để giao tiếp ra bên ngoài
EXPOSE 8080
ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]