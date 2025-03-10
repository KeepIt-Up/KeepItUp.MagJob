# Development Environment Setup

This guide provides instructions for setting up a development environment for the MagJob project.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version X.X or higher)
- [Docker](https://www.docker.com/products/docker-desktop) (version X.X or higher)
- [Node.js](https://nodejs.org/) (version X.X or higher)
- [npm](https://www.npmjs.com/) (version X.X or higher)
- IDE of your choice (Visual Studio, VS Code, Rider, etc.)

## Getting the Source Code

```bash
git clone https://github.com/your-organization/magjob.git
cd magjob
```

## Environment Configuration

1. Copy the example environment file:
   ```bash
   cp .env.example .env
   ```

2. Update the `.env` file with your local configuration values.

## Running with Docker Compose

The easiest way to run the entire application stack is using Docker Compose:

```bash
docker-compose up -d
```

This will start all the necessary services:
- Client Web Application
- API Gateway
- Organizations API
- Keycloak
- Database
- Other dependencies

## Running Individual Components

### Client Web Application

```bash
cd src/Client.Web
npm install
npm start
```

### API Gateway

```bash
cd src/APIGateway.Web
dotnet run
```

### Organizations API

```bash
cd src/Organizations.API
dotnet run
```

## Database Setup

*[Instructions for setting up and migrating the database]*

## Authentication Setup

*[Instructions for configuring Keycloak for local development]*

## Running Tests

```bash
dotnet test
```

## Debugging

*[Instructions for debugging different components]*

## Common Issues

*[List of common setup issues and their solutions]* 
