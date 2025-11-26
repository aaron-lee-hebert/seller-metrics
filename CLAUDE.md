# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Agent Role & Expertise

You are acting as a **Senior .NET Engineer** specializing in:
- **ASP.NET Core MVC** pattern and best practices
- **Clean Architecture** with SOLID principles
- **PostgreSQL** and **Azure Database for PostgreSQL** optimization
- **Bootstrap 5** front-end development
- **CI/CD** best practices with GitHub Actions and Azure DevOps

## Project Overview

SellerMetrics is a comprehensive inventory and financial management system for eBay resellers. The application tracks cost of goods, calculates profit margins after eBay fees, monitors monthly performance, and provides actionable business insights.

**Technology Stack:**
- .NET 9
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL / Azure Database for PostgreSQL
- Bootstrap 5
- eBay API integration for order and listing synchronization

**Core Features:**
- Inventory management with COGS tracking and home storage location tracking
- Automated eBay order and listing synchronization
- Real-time profit calculations including eBay fees
- Quarterly and annual financial summaries and reports
- Business expense tracking (supplies, fees, services)
- Mileage log for business travel (IRS-compliant)
- Responsive dashboard with key business metrics for sole proprietorship tax reporting

## Clean Architecture Structure

This project follows **Clean Architecture** principles with clear separation of concerns:

```
seller-metrics/
├── src/
│   ├── SellerMetrics.Domain/          # Core business entities, value objects, domain events
│   ├── SellerMetrics.Application/     # Use cases, DTOs, interfaces, CQRS handlers
│   ├── SellerMetrics.Infrastructure/  # EF Core, eBay API client, external services
│   ├── SellerMetrics.Web/             # ASP.NET Core MVC, Bootstrap 5 UI
│   ├── SellerMetrics.Tests/           # NUnit tests for all layers (Domain, Application, Infrastructure)
│   └── SellerMetrics.sln              # Solution file (includes all projects)
├── CLAUDE.md
├── TODO.md
└── README.md
```

**Dependency Flow:**
- `Domain` ← `Application` ← `Infrastructure` & `Web`
- Domain layer has NO dependencies on other layers
- Application layer depends only on Domain
- Infrastructure and Web depend on Application and Domain

### Domain Layer (SellerMetrics.Domain)
**Purpose:** Core business logic, entities, value objects, and domain events

**What belongs here:**
- Entities (Inventory, Order, Listing, Expense)
- Value Objects (Money, SKU, Address)
- Domain Events (OrderPlaced, InventorySold)
- Domain Exceptions
- Repository interfaces (IInventoryRepository, IOrderRepository)
- Domain service interfaces

**Rules:**
- NO dependencies on other layers or external libraries (except maybe core .NET types)
- Rich domain models with business logic and invariants
- All business rules enforced at the domain level
- Entities should validate their own state

### Application Layer (SellerMetrics.Application)
**Purpose:** Use cases, business workflows, and application logic

**What belongs here:**
- CQRS Commands and Queries
- Command/Query Handlers
- DTOs (Data Transfer Objects)
- Application service interfaces (IEbayApiClient, IEmailService)
- Validators (FluentValidation)
- Mapping profiles (AutoMapper or manual mappers)

**Rules:**
- Orchestrates domain objects to fulfill use cases
- Depends only on Domain layer
- Defines interfaces for infrastructure concerns (repositories, external APIs)
- No direct dependency on UI or Infrastructure implementations

### Infrastructure Layer (SellerMetrics.Infrastructure)
**Purpose:** Implements external concerns and data access

**What belongs here:**
- EF Core DbContext and entity configurations
- Repository implementations
- eBay API client implementation
- Background job services (Hangfire jobs)
- Email service implementation
- File storage services
- External API integrations

**Rules:**
- Implements interfaces defined in Application/Domain
- Contains all EF Core configuration (fluent API, migrations)
- Handles all external I/O operations
- No business logic (only infrastructure concerns)

### Web Layer (SellerMetrics.Web)
**Purpose:** User interface, controllers, views, and presentation logic

**What belongs here:**
- MVC Controllers
- Razor Views with Bootstrap 5
- ViewModels (presentation-specific models)
- Middleware
- Filters and Action Filters
- Startup/Program configuration
- JavaScript files for client-side interactions

**Rules:**
- Controllers are thin - delegate to Application layer handlers
- ViewModels for presentation concerns only
- No business logic in controllers or views
- Dependency injection wires up Application/Infrastructure

## SOLID Principles Enforcement

All code changes MUST adhere to SOLID principles:

### Single Responsibility Principle (SRP)
- Each class should have ONE reason to change
- Controllers handle HTTP concerns only, delegate to handlers
- Repositories handle data access only
- Services handle one specific domain concern

**Example:**
```csharp
// GOOD: Single responsibility
public class InventoryService
{
    public void AddInventory(Inventory item) { ... }
}

// BAD: Multiple responsibilities
public class InventoryService
{
    public void AddInventory(Inventory item) { ... }
    public void SendEmailNotification() { ... }  // Email is separate concern
    public void LogToDatabase() { ... }          // Logging is separate concern
}
```

### Open/Closed Principle (OCP)
- Classes open for extension, closed for modification
- Use interfaces and abstract classes for extensibility
- Favor composition over inheritance
- Use strategy pattern for varying behaviors

### Liskov Substitution Principle (LSP)
- Derived classes must be substitutable for base classes
- Interface implementations must honor contracts
- Don't throw NotImplementedException in interface implementations

### Interface Segregation Principle (ISP)
- Many specific interfaces better than one general interface
- Clients shouldn't depend on methods they don't use
- Keep interfaces focused and cohesive

**Example:**
```csharp
// GOOD: Segregated interfaces
public interface IInventoryReader
{
    Task<Inventory> GetByIdAsync(int id);
}

public interface IInventoryWriter
{
    Task AddAsync(Inventory item);
}

// BAD: Fat interface
public interface IInventoryRepository
{
    Task<Inventory> GetByIdAsync(int id);
    Task AddAsync(Inventory item);
    Task UpdateAsync(Inventory item);
    Task DeleteAsync(int id);
    Task BulkImportAsync(List<Inventory> items);
    Task ExportToCsvAsync(string path);  // Too many responsibilities
}
```

### Dependency Inversion Principle (DIP)
- Depend on abstractions, not concretions
- High-level modules shouldn't depend on low-level modules
- Use dependency injection throughout
- Define interfaces in Application layer, implement in Infrastructure

## Development Commands

**Build and Run:**
```bash
# From repository root
dotnet build src/SellerMetrics.sln                    # Build the solution
dotnet run --project src/SellerMetrics.Web            # Run the web application
dotnet watch --project src/SellerMetrics.Web          # Run with hot reload
```

**Testing (NUnit):**
```bash
# From repository root
dotnet test                                           # Run all tests
dotnet test --filter "FullyQualifiedName~<TestName>"  # Run specific test
dotnet test --logger "console;verbosity=detailed"     # Run with detailed output
dotnet test /p:CollectCoverage=true                   # Run with code coverage
```

**Database Migrations:**
```bash
# Run from repository root, targeting Infrastructure project
dotnet ef migrations add <MigrationName> --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
dotnet ef database update --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
dotnet ef migrations remove --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
dotnet ef database drop --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
```

**User Secrets (Development):**
```bash
# From repository root
dotnet user-secrets init --project src/SellerMetrics.Web
dotnet user-secrets set "EbayApi:ClientId" "your-client-id" --project src/SellerMetrics.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string" --project src/SellerMetrics.Web
dotnet user-secrets list --project src/SellerMetrics.Web
```

## Code Quality Standards

### Naming Conventions
- **PascalCase:** Classes, methods, properties, public fields
- **camelCase:** Private fields (prefix with `_`), local variables, parameters
- **Interfaces:** Prefix with `I` (IInventoryRepository)
- **Async methods:** Suffix with `Async` (GetInventoryAsync)
- **Test methods:** Use descriptive names (AddInventory_WithValidData_ShouldSucceed)

### MVC Controller Best Practices
- Keep controllers thin - use command/query handlers
- Return appropriate HTTP status codes (Ok, Created, NotFound, BadRequest)
- Use [ApiController] attribute for API endpoints
- Validate input using FluentValidation or DataAnnotations
- Use async/await for all I/O operations
- Handle exceptions with middleware, not try/catch in every action

**Example:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetInventory), new { id = result.Id }, result);
}
```

### Entity Framework Core Best Practices
- Use fluent API for entity configuration (not data annotations)
- Always use `.AsNoTracking()` for read-only queries
- Use `.Include()` explicitly, avoid lazy loading in production
- Use projections (`.Select()`) for queries that don't need full entities
- Add indexes for foreign keys and frequently queried fields
- Use value converters for Value Objects
- Implement audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)

**Example entity configuration:**
```csharp
public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.SKU).IsRequired().HasMaxLength(50);
        builder.HasIndex(i => i.SKU).IsUnique();

        // Value Object conversion
        builder.OwnsOne(i => i.PurchasePrice, price =>
        {
            price.Property(p => p.Amount).HasColumnName("PurchaseAmount").HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        });
    }
}
```

### Bootstrap 5 & Front-End Standards
- Use Bootstrap 5 utility classes for spacing, layout, and responsive design
- Implement responsive design mobile-first
- Use Bootstrap components (cards, modals, navs, forms)
- Add custom CSS sparingly - prefer Bootstrap utilities
- Use data-bs-* attributes for Bootstrap JavaScript components
- Implement client-side validation with Bootstrap validation styles
- Use AJAX with fetch API or jQuery for dynamic interactions
- Follow accessibility best practices (ARIA labels, semantic HTML)

### Error Handling & Logging
- Use structured logging with Microsoft.Extensions.Logging
- Log at appropriate levels (Debug, Information, Warning, Error, Critical)
- Include correlation IDs for distributed tracing
- Don't log sensitive data (passwords, API keys, PII)
- Use global exception handler middleware
- Return problem details (RFC 7807) for API errors

### Security Best Practices
- **NEVER** commit secrets, API keys, or connection strings
- Use user secrets for development, Azure Key Vault for production
- Validate and sanitize all user input
- Use parameterized queries (EF Core does this automatically)
- Implement CSRF protection on forms
- Use HTTPS everywhere (HSTS headers)
- Implement proper authentication and authorization
- Follow principle of least privilege for database permissions

## Sole Proprietorship Tax Compliance

This application is designed to support **sole proprietorship** tax reporting requirements:

**IRS Schedule C Compliance:**
- Track gross receipts/sales by platform (eBay, Direct Sales)
- Categorize all business expenses for Schedule C Part II
- Generate quarterly and annual profit/loss reports
- Maintain detailed records for 7+ years (IRS audit requirement)

**Mileage Log Requirements (IRS-compliant):**
- Date of trip
- Business purpose (e.g., "Post office - ship eBay orders", "Thrift store sourcing")
- Starting location and destination
- Miles driven
- Calculate deduction using current IRS mileage rate (e.g., $0.67/mile for 2024)
- Support both actual expense method and standard mileage method

**Business Expense Categories:**
- Shipping supplies (boxes, tape, bubble wrap, labels)
- eBay/PayPal fees (already tracked per transaction)
- Office supplies
- Mileage/vehicle expenses
- Storage/warehouse costs (if applicable)
- Professional services (accounting, legal)
- Advertising/marketing
- Other business expenses

**Quarterly Tax Estimates:**
- Calculate estimated tax payments based on quarterly profit
- Track quarterly payments made
- Alert when quarterly filing deadlines approach (April 15, June 15, Sept 15, Jan 15)

## Inventory Storage Tracking

**Home Inventory Location:**
- Track physical location of inventory items in home (e.g., "Garage Shelf A", "Basement Bin 3", "Closet - Top Shelf")
- Support hierarchical locations (Room > Storage Unit > Bin/Shelf)
- Quick search/filter to locate items when orders placed
- Mark items as "Listed" vs "Unlisted" for storage optimization

## eBay API Integration

**Authentication:**
- Use OAuth 2.0 for eBay API authentication
- Store refresh tokens securely (Key Vault)
- Implement token refresh logic

**Rate Limiting:**
- Implement retry policies with exponential backoff (Polly library)
- Track API call quotas
- Use background jobs for bulk synchronization

**Error Handling:**
- Handle eBay API errors gracefully (429 Too Many Requests, 401 Unauthorized)
- Log all API errors with request/response details
- Implement circuit breaker pattern for API failures

**Data to Sync from eBay:**
- Order details (order ID, date, buyer info, gross sale price)
- Fees breakdown (final value fee, payment processing fee)
- Shipping costs
- Item sold (match to inventory SKU)
- Listing status (active, ended, sold)

## Financial Calculations

**Critical Rules:**
- **ALWAYS** use `decimal` type for monetary values (NEVER float or double)
- Account for ALL eBay fees in profit calculations:
  - Final value fee (percentage of sale price)
  - Payment processing fee
  - Listing fees (if applicable)
  - Promoted listing fees (if applicable)
- COGS must be tracked at the item level
- Round monetary values to 2 decimal places for display
- Store currency code with monetary amounts (use Money value object)

**Profit Calculation (matching spreadsheet):**
```
Gross Sale = Sale Price from eBay
Fees = eBay Final Value Fee + Payment Processing Fee
Shipping = Actual shipping cost paid
Net Payout = Gross Sale - Fees - Shipping

Profit = Net Payout - COGS
```

**Tax Reporting:**
- Track all sales by quarter (Q3 2025, Q4 2025, etc.)
- Separate eBay sales from direct sales
- Business expenses must be categorized for Schedule C
- Mileage log must include: Date, Purpose, Starting Location, Destination, Miles
- Generate quarterly summaries for estimated tax payments

## Testing Strategy

**Test Project Organization (SellerMetrics.Tests):**
- Single NUnit test project for all layers
- Organize tests by namespace/folder:
  - `SellerMetrics.Tests/Domain/` - Domain entity and business logic tests
  - `SellerMetrics.Tests/Application/` - Command/query handler tests
  - `SellerMetrics.Tests/Infrastructure/` - Integration tests for repositories, EF Core, API clients

**Unit Tests:**
- Test domain entities and business logic in isolation
- Test command/query handlers with mocked dependencies
- Use **NUnit** as the test framework
- Use Moq or NSubstitute for mocking
- Aim for >80% code coverage on business logic

**Integration Tests:**
- Test repository implementations with real database (in-memory or TestContainers)
- Test EF Core configurations and migrations
- Test API client integrations (with mock server or test endpoints)

**Test Naming (NUnit):**
```csharp
// Domain tests
namespace SellerMetrics.Tests.Domain
{
    [TestFixture]
    public class InventoryTests
    {
        [Test]
        public void AddInventory_WithNegativeCost_ShouldThrowException()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}

// Application tests with test cases
namespace SellerMetrics.Tests.Application
{
    [TestFixture]
    public class ProfitCalculationTests
    {
        [TestCase(100, 85, 15)]  // Sale price, costs, expected profit
        [TestCase(50, 40, 10)]
        public void CalculateProfit_WithValidInputs_ReturnsCorrectProfit(decimal salePrice, decimal costs, decimal expected)
        {
            // NUnit parameterized tests
        }
    }
}

// Infrastructure integration tests
namespace SellerMetrics.Tests.Infrastructure
{
    [TestFixture]
    public class InventoryRepositoryTests
    {
        // Integration tests using TestContainers or in-memory database
    }
}
```

## CI/CD Pipeline (GitHub Actions + Azure)

**CI Pipeline (on PR):**
- Restore dependencies
- Build solution
- Run all tests
- Check code coverage (fail if below threshold)
- Run static code analysis (optional: SonarCloud)

**CD Pipeline (on merge to main):**
- Build and publish application
- Run database migrations (via migration bundle or SQL script)
- Deploy to Azure App Service (Staging first, then Production)
- Run smoke tests against deployed environment
- Notify team of deployment status

**Azure Resources:**
- Azure App Service (Web app hosting)
- Azure Database for PostgreSQL (Database)
- Azure Key Vault (Secrets management)
- Application Insights (Monitoring and logging)

## Performance Optimization

- Use caching for frequently accessed, rarely changed data (IMemoryCache or IDistributedCache)
- Implement pagination on all list endpoints (Page size: 20-50 items)
- Use database indexes strategically (avoid over-indexing)
- Profile slow queries with EF Core logging or PostgreSQL EXPLAIN ANALYZE
- Use async/await for all I/O operations
- Consider read replicas for reporting queries (if needed at scale)
- Use projection queries (`.Select()`) instead of loading full entities when possible

## Documentation Standards

- Use XML documentation comments for all public APIs
- Keep README.md updated with setup instructions
- Document architectural decisions in ADR (Architecture Decision Records)
- Update TODO.md as tasks are completed
- Inline comments for complex business logic only (code should be self-documenting)
