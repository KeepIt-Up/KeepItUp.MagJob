# KeepItUp.MagJob.Identity

## Overview

This module provides identity and organization management for the MagJob application, implementing Domain-Driven Design patterns.

## Architecture

The solution follows Clean Architecture principles:

- **Core**: Contains domain entities, aggregates, events, and business logic
- **Infrastructure**: Contains implementation details (database, external services, etc.)
- **API**: Exposes the functionality via REST endpoints

## Key Domain Components

### Organization Aggregate

The `Organization` aggregate root manages:

- Organization details (name, description, active status)
- Members and their roles
- Permissions and access control
- Domain events for significant state changes

### Domain Events

The system uses domain events to decouple operations and maintain a clear history of state changes:

- `OrganizationCreatedEvent`: Triggered when a new organization is created
- `OrganizationOwnerInitializedEvent`: Triggered when an owner is assigned to an organization
- `OrganizationRolesInitializedEvent`: Triggered when default roles are set up
- `MemberAddedEvent`: Triggered when a new member is added
- And more...

### Event Handlers

Domain event handlers react to events and perform additional operations:

- `OrganizationCreatedEventHandler`: Sends welcome emails, performs additional setup
- `OrganizationOwnerInitializedEventHandler`: Sets up owner-specific configurations
- And more...

## Design Patterns

The solution implements several key design patterns:

1. **Factory Methods**: Entities use static factory methods for creation (e.g., `Organization.Create`)
2. **Domain Events**: Entities raise domain events for significant state changes
3. **Repository Pattern**: Data access is abstracted through repositories
4. **Dependency Injection**: Services are registered and injected where needed

## Validation Approach

1. **Entity Validation**: Business rules are enforced in the domain model
2. **Input Validation**: Input data is validated at the API boundary
3. **Invariants**: Domain entities maintain their invariants during state changes

## Testing Strategy

1. **Unit Tests**: Focus on domain logic, using mocks for external dependencies
2. **Integration Tests**: Test components working together
3. **End-to-End Tests**: Test complete flows through the system

## Key Technical Decisions

1. Using EF Core for data access with a repository abstraction
2. Implementing CQRS pattern for separation of reading and writing operations
3. Using domain events for loose coupling between components
4. Using a shared kernel library for common abstractions

## Getting Started

1. Ensure you have the .NET 7.0 SDK installed
2. Update the connection string in `appsettings.json`
3. Run database migrations: `dotnet ef database update`
4. Start the API project: `dotnet run --project src/KeepItUp.MagJob.Identity.API`

## Development Guidelines

Please refer to the `DesignGuidelines.md` file for detailed coding and architecture guidelines.
