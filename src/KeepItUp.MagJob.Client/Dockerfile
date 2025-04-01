# Stage 1: Build the Angular application
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json
COPY src/KeepItUp.MagJob.Client/package*.json ./

# Install dependencies
RUN npm ci

# Copy the rest of the application code
COPY src/KeepItUp.MagJob.Client/ ./


# Build the application
RUN npm run build:docker

# Stage 2: Serve the application with Nginx
FROM nginx:alpine

# Copy the build output to replace the default nginx contents
COPY --from=build /app/dist/keepitup.magjob.client/browser /usr/share/nginx/html

# Copy custom nginx configuration
COPY src/KeepItUp.MagJob.Client/nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# When the container starts, nginx will start and serve the application
CMD ["nginx", "-g", "daemon off;"]
