# DistLib

[![.NET](https://github.com/yourusername/DistLib/actions/workflows/dotnet.yml/badge.svg)](https://github.com/yourusername/DistLib/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/DistLib.Core.svg)](https://www.nuget.org/packages/DistLib.Core)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A .NET class library designed to support clean architecture, Domain-Driven Design (DDD), event-based choreography, messaging-based saga patterns, and other enterprise patterns for microservices and modular monoliths.

## 🚀 Features

### Core Abstractions
- **CQRS**: Command and Query Responsibility Segregation with clean abstractions
  - **Commands**: Interface-based command definitions with built-in validation support
  - **Queries**: Interface-based query definitions with filtering, pagination, and sorting capabilities
  - **Dispatchers**: Command and query dispatchers for seamless request handling
  - **Pipeline**: Built-in validation, exception handling, and side-effect rollback behaviors
  - **Registration**: Easy service registration and dependency injection setup
- **Domain Events**: Event-driven architecture support with domain event dispatching
- **Result Pattern**: Functional programming monad for error handling and composition with Result<T> and Error types
- **DateTime Abstraction**: Testable datetime operations
- **Environment Provider**: Environment-specific configuration management

> **Note**: MediatR is currently used under the hood for the CQRS implementation, providing a robust foundation for the command and query pipeline.

### MediatR Integration
The `DistLib.Requests.MediaR` package provides MediatR as the CQRS implementation. To use it:

```csharp
// Register the MediatR request pipeline
services.UseMediaRRequestPipeline(new[] { ApplicationAssembly.Assembly });

// This automatically registers:
// - MediatR with your assemblies
// - Command and Query dispatchers
// - Pipeline behaviors (validation, exception handling, side-effect rollback)
```

**Handler Interfaces**: Use `ICommandHandler<TCommand, TResult>` and `IQueryHandler<TQuery, TResult>` for your handlers.

See `Extensions.cs` in the MediatR project for the complete setup details.

### Web API Support
The `DistLib.WebApi` package provides utilities for building the presentation layer with minimal APIs, command/query composition, and response mapping:

#### JSON Configuration
- **Custom JSON Settings**: Configures JSON serialization with camelCase naming, case-insensitive properties, and enum conversion
- **HTTP Context Access**: Provides access to the current HTTP context for request/response handling

#### Model Binding Utilities
- **Expression-Based Binding**: Type-safe property binding using lambda expressions for composing commands and queries
- **ID Generation**: Automatic GUID generation and binding for entity properties
- **String ID Support**: GUID-to-string conversion for string-based identifiers

```csharp
// Example usage for composing commands/queries
var user = new User();
user.Bind(u => u.Name, "Alice");
user.BindId(u => u.Id);           // Generates new Guid
user.BindId(u => u.StringId);     // Generates new string ID
```

#### Result Mapping for Minimal APIs
- **HTTP Result Conversion**: Automatically maps `Result<T>` types to appropriate HTTP responses
- **Status Code Mapping**: Maps result statuses to correct HTTP status codes (200, 400, 401, 403, 404, 500)
- **Minimal API Support**: Designed for building clean, minimal API endpoints

```csharp
// In your minimal API endpoint
app.MapGet("/users/{id}", async (Guid id, IQueryDispatcher dispatcher) =>
{
    var query = new GetUserQuery { Id = id };
    var result = await dispatcher.DispatchAsync(query);
    return result.ToApiResult(); // Automatically maps to HTTP response
});
```

## 📦 NuGet Packages

| Package | Description | Version |
|---------|-------------|---------|
| [DistLib.Core](https://www.nuget.org/packages/DistLib.Core) | Core abstractions and utilities | ![NuGet](https://img.shields.io/nuget/v/DistLib.Core.svg) |
| [DistLib.Requests.MediatR](https://www.nuget.org/packages/DistLib.Requests.MediatR) | MediatR integration package | ![NuGet](https://img.shields.io/nuget/v/DistLib.Requests.MediatR.svg) |
| [DistLib.WebApi](https://www.nuget.org/packages/DistLib.WebApi) | Web API support package | ![NuGet](https://img.shields.io/nuget/v/DistLib.WebApi.svg) |

## 🏗️ Architecture

DistLib follows clean architecture principles and provides building blocks for:

- **Clean Architecture**: Separation of concerns with clear boundaries
- **Domain-Driven Design**: Domain events, aggregates, and business logic encapsulation
- **CQRS**: Command and Query Responsibility Segregation
- **Event Sourcing**: Event-driven architecture with domain event dispatching
- **Saga Pattern**: Long-running business process management
- **Microservices**: Service communication and coordination patterns

## 🚀 Quick Start

### Installation

```bash
# Core package
dotnet add package DistLib.Core

# MediatR integration
dotnet add package DistLib.Requests.MediatR

# Web API support
dotnet add package DistLib.WebApi
```

### Basic Usage

```csharp
using DistLib;
using DistLib.WebApi;
using DistLib.Requests.MediaR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDistLib(builder.Configuration)
    .AddInMemoryDomainEventDispatcher(new Assembly[] { ApplicationAssembly.Assembly })
    .UseMediaRRequestPipeline(new Assembly[] { ApplicationAssembly.Assembly });

// Define commands and queries
public sealed record GetUser(Guid UserId) : IQuery<Result>;
public sealed record CreateUser(Guid UserId, string Email, string Password) : ICommand<Result>;

// Define handlers
public sealed class GetUserHandler : IQueryHandler<GetUser, Result>
{
    public async Task<Result> HandleAsync(GetUser query, CancellationToken cancellationToken)
    {
        // Your business logic here
        return Result.Success();
    }
}

public sealed class CreateUserHandler : ICommandHandler<CreateUser, Result>
{
    public async Task<Result> HandleAsync(CreateUser command, CancellationToken cancellationToken)
    {
        // Your business logic here
        return Result.Success();
    }
}

// Minimal API endpoints
app.MapGet("users/{userId}", async ([FromBody] GetUser request, IQueryDispatcher dispatcher) =>
{
    var result = await dispatcher.DispatchAsync(request);
    return result.ToApiResult();
}).RequireAuthorization("Admin");

app.MapPost("create-user", async ([FromBody] CreateUser cmd, ICommandDispatcher dispatcher, CancellationToken ct) =>
{
    var userId = Guid.NewGuid();
    var result = await dispatcher.DispatchAsync<CreateUser, Result>(
        cmd.Bind(u => u.UserId, userId), ct);
    return result.ToApiResult(() => Results.Created("identity/me", (object?)userId));
});
```

#### Assembly-Based Registration
The assembly-based registration approach provides flexibility for organizing commands and queries across different assemblies within the same solution. This enables:
- **Domain Isolation**: Separate business domains into different assemblies
- **Reporting Access**: Isolate read models and reporting queries
- **Reduced Coupling**: Minimize dependencies between different parts of your application

## 📚 Documentation

- [Getting Started Guide](docs/getting-started.md)
- [Architecture Overview](docs/architecture.md)
- [Patterns and Examples](docs/patterns.md)
- [API Reference](docs/api-reference.md)

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details on how to:

- Report bugs
- Request features
- Submit pull requests
- Set up your development environment

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🏆 Acknowledgments

- Inspired by clean architecture principles
- Built on top of excellent .NET libraries like MediatR, FluentValidation, and Scrutor
- Community-driven development and feedback

## 📞 Support

- 📖 [Documentation](docs/)
- 🐛 [Issues](https://github.com/yourusername/DistLib/issues)
- 💬 [Discussions](https://github.com/yourusername/DistLib/discussions)
- 📧 [Email Support](mailto:support@distlib.dev)

---

**Made with ❤️ by the .NET community**
