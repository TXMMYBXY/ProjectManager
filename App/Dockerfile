FROM node:18 AS build
WORKDIR /app

ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

# Копируем только файлы package.json и package-lock.json для установки зависимостей
COPY App/package*.json ./

RUN npm ci --silent

# Копируем остальные файлы
COPY App/ .

RUN npm run build --if-present

# Используем легковесный node-образ для раздачи статических файлов через пакет 'serve'
FROM node:18-alpine
WORKDIR /app
COPY --from=build /app/dist ./dist
RUN npm install -g serve --silent
EXPOSE 80
CMD ["serve", "-s", "dist", "-l", "80"]