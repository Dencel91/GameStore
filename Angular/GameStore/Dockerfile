# Stage 1: Build the app
FROM node:20-alpine AS builder
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build -- --configuration production

# Stage 2: Serve with nginx
FROM nginx:alpine
COPY --from=builder /app/dist/game-store/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf