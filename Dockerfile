# GIAI ĐOẠN 1: Dùng bộ SDK nặng để Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file cấu hình project (.csproj) vào trước để tối ưu hóa việc tải thư viện
COPY ["UrlShortener.API/UrlShortener.API.csproj", "UrlShortener.API/"]
COPY ["UrlShortener.Services/UrlShortener.Services.csproj", "UrlShortener.Services/"]
COPY ["UrlShortener.Data/UrlShortener.Data.csproj", "UrlShortener.Data/"]
COPY ["UrlShortener.Common/UrlShortener.Common.csproj", "UrlShortener.Common/"]

# Chạy lệnh khôi phục (tải) các thư viện
RUN dotnet restore "UrlShortener.API/UrlShortener.API.csproj"

# Copy toàn bộ mã nguồn còn lại vào
COPY . .

# Build dự án
WORKDIR "/src/UrlShortener.API"
RUN dotnet build "UrlShortener.API.csproj" -c Release -o /app/build

# GIAI ĐOẠN 2: Đóng gói (Publish)
FROM build AS publish
RUN dotnet publish "UrlShortener.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# GIAI ĐOẠN 3: Dùng bộ Runtime siêu nhẹ để chạy ứng dụng (Môi trường Production)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Chỉ copy phần file đã được đóng gói từ Giai đoạn 2 sang
COPY --from=publish /app/publish .

# Lệnh khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "UrlShortener.API.dll"]