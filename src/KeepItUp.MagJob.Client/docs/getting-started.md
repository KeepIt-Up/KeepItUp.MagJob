# Getting Started

This guide will help you set up and run the MagJob client application for development.

## Table of Contents

- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Development Server](#development-server)
- [Building for Production](#building-for-production)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)

## Prerequisites

Before you begin, ensure you have the following installed:

- [Node.js](https://nodejs.org/) (v18 or later)
- [npm](https://www.npmjs.com/) (v9 or later)
- [Angular CLI](https://angular.io/cli) (v19)

## Installation

1. Clone the repository:

```bash
git clone <repository-url>
cd MagJob
```

2. Navigate to the client directory:

```bash
cd src/KeepItUp.MagJob.Client
```

3. Install dependencies:

```bash
npm install
```

## Configuration

The application uses environment-specific configuration files located in the `src/environments` directory:

- `environment.ts`: Development configuration
- `environment.docker.ts`: Docker development configuration
- `environment.prod.ts`: Production configuration

Before running the application locally, you need to create your own local environment configuration:

1. Copy the environment template file:

```bash
cp src/environments/environment.local.dist.ts src/environments/environment.local.ts
```

2. Edit the `environment.local.ts` file to set your local API URLs and other configuration values.

Note: The `environment.local.ts` file is included in `.gitignore` to prevent committing your local configuration to the repository.

## Running the Application

### Development Server

To start the development server with the local configuration:

```bash
npm run local
```

This will start the application on `http://localhost:4200/`.

### Docker Development

To start the development server with the Docker configuration:

```bash
npm run docker
```

This is useful when running the application as part of the Docker Compose setup.

### Development Build

To build the application for development:

```bash
npm run build:dev
```

The build artifacts will be stored in the `dist/` directory.

## Development Workflow

### Code Formatting

The project uses ESLint and Prettier for code formatting. To format your code:

```bash
npm run format
```

To check if your code is properly formatted:

```bash
npm run format:check
```

### Linting

To lint your code:

```bash
npm run lint
```

### Testing

To run unit tests:

```bash
npm test
```

## Project Structure

The client application follows a modular architecture:

- `src/app/core`: Core functionality used throughout the application
- `src/app/features`: Feature modules with domain-specific functionality
- `src/app/pages`: Page components that compose features
- `src/app/shared`: Shared components, services, and utilities

For more details, see the [Project Structure](./project-structure.md) documentation.

## Styling

The application uses TailwindCSS for styling. Custom components and utility classes are defined in the global styles.

For more details, see the [Style Guide](./style-guide.md) documentation.

## Troubleshooting

### Common Issues

#### Build Errors

If you encounter build errors:

1. Ensure all dependencies are installed: `npm install`
2. Clear the Angular cache: `ng cache clean`
3. Check for TypeScript errors in your code

### Getting Help

If you need further assistance:

1. Check the project documentation
2. Review the Angular documentation: [https://angular.io/docs](https://angular.io/docs)
3. Contact the development team
