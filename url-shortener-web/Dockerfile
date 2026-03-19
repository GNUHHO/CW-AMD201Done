# --- BƯỚC 1: MÔI TRƯỜNG BUILD ---
# Dùng Node.js để tải thư viện và build code Vue.js thành HTML/CSS/JS thuần
FROM node:20-alpine AS build
WORKDIR /app

# Copy package.json và cài đặt thư viện (như Bootstrap, Vue...)
COPY package*.json ./
RUN npm install

# Copy toàn bộ code và build ra thư mục /dist
COPY . .
RUN npm run build

# --- BƯỚC 2: MÔI TRƯỜNG CHẠY (Nginx) ---
# Dùng máy chủ Web Nginx siêu nhẹ để hiển thị giao diện cho người dùng
FROM nginx:alpine AS final

# Copy kết quả đã build ở bước 1 sang thư mục mặc định của Nginx
COPY --from=build /app/dist /usr/share/nginx/html

# Mở cửa số 80
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]