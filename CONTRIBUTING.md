# Contributing to DistLib

Thank you for your interest in contributing to DistLib! This document provides guidelines and information for contributors.

## 🤝 How to Contribute

There are several ways you can contribute to DistLib:

- 🐛 **Report bugs** - Help us identify and fix issues
- 💡 **Suggest features** - Propose new functionality or improvements
- 📝 **Improve documentation** - Help make our docs clearer and more comprehensive
- 🔧 **Submit code** - Fix bugs, implement features, or improve existing code
- 🧪 **Write tests** - Help ensure code quality and reliability
- 📢 **Spread the word** - Share DistLib with your network

## 🚀 Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Development Environment Setup

1. **Fork the repository**
   ```bash
   # Clone your fork
   git clone https://github.com/yourusername/DistLib.git
   cd DistLib
   
   # Add the upstream remote
   git remote add upstream https://github.com/originalusername/DistLib.git
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

## 📋 Development Workflow

### 1. Create a Feature Branch

```bash
# Create and checkout a new branch
git checkout -b feature/your-feature-name

# Or for bug fixes
git checkout -b bugfix/issue-description
```

**Branch Naming Convention:**
- `feature/` - New features
- `bugfix/` - Bug fixes
- `docs/` - Documentation updates
- `refactor/` - Code refactoring
- `test/` - Test additions/improvements

### 2. Make Your Changes

- Follow the [coding standards](#coding-standards) below
- Write or update tests for your changes
- Update documentation if needed
- Keep commits focused and atomic

### 3. Test Your Changes

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test DistLib.Core.Tests

# Run tests with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### 4. Commit Your Changes

```bash
# Stage your changes
git add .

# Commit with a descriptive message
git commit -m "feat: add new validation behavior for commands

- Implements IValidationBehavior interface
- Adds FluentValidation integration
- Includes unit tests for validation logic"
```

**Commit Message Format:**
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

**Types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation changes
- `style` - Code style changes (formatting, etc.)
- `refactor` - Code refactoring
- `test` - Test additions/changes
- `chore` - Build process or auxiliary tool changes

### 5. Push and Create a Pull Request

```bash
# Push your branch
git push origin feature/your-feature-name
```

Then create a Pull Request on GitHub with:
- Clear title describing the change
- Detailed description of what was changed and why
- Reference to any related issues
- Screenshots for UI changes (if applicable)

## 🏗️ Project Structure

```
DistLib/
├── DistLib.Core/                 # Core abstractions and utilities
│   ├── Abstractions/            # Base interfaces and classes
│   ├── ApplicationFactory/      # Service registration and configuration
│   └── ApplicationProperties/   # Application options and settings
├── DistLib.Requests.MediatR/    # MediatR integration package
│   ├── Behaviours/             # Pipeline behaviors
│   └── Dispatchers/            # Command and query dispatchers
├── DistLib.WebApi/              # Web API support package
├── tests/                       # Test projects (to be added)
└── docs/                        # Documentation (to be added)
```

## 📝 Coding Standards

### C# Code Style

- Use **PascalCase** for public members, classes, and methods
- Use **camelCase** for private fields and local variables
- Use **UPPER_CASE** for constants
- Prefer `var` for local variable declarations when the type is obvious
- Use meaningful names that clearly express intent
- Keep methods focused and single-purpose (max 20-30 lines)
- Use XML documentation for public APIs

### Example

```csharp
/// <summary>
/// Handles the creation of new user accounts.
/// </summary>
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IDomainEventDispatcher _eventDispatcher;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IDomainEventDispatcher eventDispatcher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
    }

    public async Task<Result<Guid>> Handle(
        CreateUserCommand request, 
        CancellationToken cancellationToken)
    {
        // Implementation here
    }
}
```

### Testing Standards

- Write unit tests for all public methods
- Use descriptive test names that explain the scenario and expected outcome
- Follow the Arrange-Act-Assert pattern
- Mock external dependencies
- Aim for high test coverage (80%+)

```csharp
[Fact]
public async Task Handle_ValidCreateUserCommand_ReturnsSuccessResultWithUserId()
{
    // Arrange
    var command = new CreateUserCommand { Name = "John Doe", Email = "john@example.com" };
    var handler = new CreateUserCommandHandler(_mockRepository.Object, _mockDispatcher.Object);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Should().NotBeEmpty();
}
```

## 🐛 Reporting Issues

When reporting bugs, please include:

1. **Clear description** of the problem
2. **Steps to reproduce** the issue
3. **Expected vs actual behavior**
4. **Environment details** (.NET version, OS, etc.)
5. **Code samples** if applicable
6. **Stack traces** for exceptions

**Issue Template:**
```markdown
## Bug Description
[Clear description of the bug]

## Steps to Reproduce
1. [Step 1]
2. [Step 2]
3. [Step 3]

## Expected Behavior
[What you expected to happen]

## Actual Behavior
[What actually happened]

## Environment
- .NET Version: [e.g., .NET 8.0]
- OS: [e.g., Windows 11, macOS 14, Ubuntu 22.04]
- DistLib Version: [e.g., 1.0.1]

## Additional Information
[Any other relevant information]
```

## 💡 Suggesting Features

For feature requests, please:

1. **Describe the problem** you're trying to solve
2. **Explain your proposed solution**
3. **Provide use cases** and examples
4. **Consider alternatives** you've explored
5. **Show how it fits** with DistLib's architecture

## 🔍 Code Review Process

1. **Automated Checks** - CI/CD pipeline runs tests and style checks
2. **Review Request** - At least one maintainer must approve
3. **Feedback Loop** - Address any review comments
4. **Merge** - Once approved, your PR will be merged

## 📚 Documentation

When contributing code, please:

- Update relevant documentation
- Add XML documentation for public APIs
- Include usage examples
- Update README if adding new features

## 🏷️ Versioning

DistLib follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version for incompatible API changes
- **MINOR** version for backwards-compatible functionality
- **PATCH** version for backwards-compatible bug fixes

## 🎯 Areas for Contribution

We're particularly interested in contributions for:

- **Testing**: Unit tests, integration tests, performance tests
- **Documentation**: API docs, tutorials, examples
- **Performance**: Optimizations and benchmarking
- **New Patterns**: Additional enterprise patterns and abstractions
- **Tooling**: Build scripts, CI/CD improvements

## 📞 Getting Help

- **GitHub Issues**: For bugs and feature requests
- **GitHub Discussions**: For questions and general discussion
- **Pull Requests**: For code contributions
- **Email**: For private or sensitive matters

## 🙏 Recognition

Contributors will be recognized in:

- GitHub contributors list
- Release notes
- Project documentation
- Community acknowledgments

Thank you for contributing to DistLib! 🎉
