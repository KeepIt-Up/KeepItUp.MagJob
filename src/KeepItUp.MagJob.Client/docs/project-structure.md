# Project Structure

The MagJob client application follows a modular architecture to promote maintainability, scalability, and separation of concerns. This document outlines the organization of the codebase.

## Table of Contents

- [Directory Structure](#directory-structure)
- [Core Module](#core-module)
- [Features Module](#features-module)
- [Shared Module](#shared-module)
- [Pages](#pages)
- [Assets](#assets)
- [Environments](#environments)

## Directory Structure

```
src/
├── app/
│   ├── core/           # Core functionality used throughout the application
│   ├── features/       # Feature modules with domain-specific functionality
│   ├── pages/          # Page components that compose features
│   ├── shared/         # Shared components, services, and utilities
│   ├── app.component.ts # Root component
│   ├── app.config.ts   # Application configuration
│   └── app.routes.ts   # Application routes
├── assets/             # Static assets (images, fonts, etc.)
├── environments/       # Environment-specific configuration
├── styles.scss         # Global styles
└── main.ts             # Application entry point
```

## Core Module

The `core` directory contains essential services, models, and utilities that are used throughout the application:

```
core/
├── configs/       # Application configuration
├── guards/        # Route guards for authentication and authorization
├── interceptors/  # HTTP interceptors
├── models/        # Core data models
└── services/      # Core services (authentication, API, etc.)
```

These components are typically loaded once when the application starts and are not imported by feature modules.

## Features Module

The `features` directory contains domain-specific functionality organized into feature modules:

```
features/
├── auth-test/     # Authentication testing features
├── invitations/   # Invitation management
├── members/       # Member management
├── organizations/ # Organization management
├── roles/         # Role management
└── users/         # User management
```

Each feature module encapsulates related components, services, and models for a specific domain area.

## Pages Module

The `pages` directory contains page-level components that compose features:

```
pages/
├── help/          # Help and documentation pages
├── landing/       # Landing and marketing pages
├── organization/  # Organization-related pages
└── user/          # User-related pages
```

Pages typically combine multiple feature components to create complete views.

## Shared Module

The `shared` directory contains reusable components, services, and utilities that can be imported by any feature module:

```
shared/
├── components/    # Reusable UI components
├── directives/    # Custom directives
├── models/        # Shared data models
├── pipes/         # Custom pipes
└── services/      # Shared services
```

## Module Organization

Each module (feature, page, etc.) typically follows this structure:

```
module-name/
├── components/    # Components specific to this module
├── models/        # Data models
└── services/      # Services
```

This consistent organization makes it easier to navigate and understand the codebase.
