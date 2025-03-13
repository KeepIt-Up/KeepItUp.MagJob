# Etap budowania
FROM node:20.9 AS build

# Ustawienie katalogu roboczego
WORKDIR /app

# Kopiowanie pliku package.json i package-lock.json (jeśli istnieje)
COPY package*.json ./

# Instalacja zależności
RUN npm install

# Kopiowanie całej aplikacji Angular
COPY . .

# Budowanie aplikacji w trybie development
RUN npm run build:dev

# Etap serwowania
FROM nginx:alpine

# Kopiowanie zbudowanej aplikacji do nginx
COPY --from=build /app/dist/keepitup.magjob.client/browser/ /usr/share/nginx/html/

# Kopiowanie konfiguracji nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
