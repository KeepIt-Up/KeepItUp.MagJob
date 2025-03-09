# ClientWeb

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.2.5.

## Technologies Used

### Node.js
Node.js is a JavaScript runtime environment that executes JavaScript code outside a web browser. It enables building scalable network applications using an event-driven, non-blocking I/O model. In this project, Node.js serves as the foundation for running the development tools and managing dependencies.

### Angular
Angular is a powerful TypeScript-based web application framework developed by Google. It provides:
- Component-based architecture
- Robust dependency injection
- Declarative templates
- Comprehensive routing
- State management solutions
- Development tools for building scalable web applications

## Prerequisites

Before you begin, ensure you have the following installed:
- Node.js (v20.17.0 or higher)
- npm (Node Package Manager)
- Angular CLI (`npm install -g @angular/cli`)
- Visual Studio Code (recommended)

## Installation

1. Install dependencies:
```bash
npm install
```

2. Install recommended VS Code extensions:
- Angular Language Service
- ESLint
- Prettier (optional)

## Development Setup

1. Start the development server:
```bash
npm start
```
Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

2. For Docker development:
```bash
docker-compose up
```

## Project Structure

```
src/
├── app/
│ ├── apis/ # API services and interfaces
│ ├── core/ # Core modules, guards, interceptors
│ │ ├── components/ # Core components (navbar, sidebar)
│ │ ├── config/ # Configuration files
│ │ ├── guards/ # Route guards
│ │ ├── interceptors/ # HTTP interceptors
│ │ └── services/ # Core services
│ ├── features/ # Feature modules
│ └── shared/ # Shared modules, components, services
└── assets/ # Static assets
```

## Available Commands

- `ng serve`: Start development server
- `ng build`: Build the project
- `ng test`: Execute unit tests
- `ng lint`: Run linting
- `ng e2e`: Run end-to-end tests

## Code Scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Testing

- Unit Tests: `ng test`
- E2E Tests: `ng e2e`

## Further Help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
