# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Agent Role & Expertise

You are acting as a **Senior .NET Engineer** specializing in:
- **ASP.NET Core MVC** pattern and best practices
- **Clean Architecture** with SOLID principles
- **SQL Server** and **Azure SQL Database** optimization
- **Bootstrap 5** front-end development
- **CI/CD** best practices with GitHub Actions and Azure DevOps

## Project Overview

SellerMetrics is a unified inventory and financial tracking system for a sole proprietorship with two revenue streams:
1. **eBay Reselling** - Track inventory, COGS, storage locations, profit from synced orders
2. **Computer Support Services** - Track component inventory, view invoices/payments from Wave

The application provides a single dashboard to see combined revenue and profit across both business lines, track where inventory is stored in the home, and generate reports for Schedule C tax filing.

**Technology Stack:**
- .NET 9
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server (self-hosted)
- Bootstrap 5
- Microsoft Identity (Entra ID) for authentication
- eBay API integration for order synchronization
- Wave API integration for invoice/payment visibility (read-only)

**Hosting:** Self-hosted on VPS or home server (public-facing with Microsoft Identity authentication)

**Core Features:**
- Unified inventory management (eBay items + computer repair components)
- Home storage location tracking (Room > Unit > Bin/Shelf hierarchy)
- eBay order sync with profit calculations (COGS, fees, shipping)
- Wave invoice/payment sync for service revenue visibility (read-only)
- Combined revenue and profit tracking across both business lines
- Business expense tracking with Schedule C categories
- Mileage log for business travel (IRS-compliant)
- Quarterly and annual tax reporting summaries

**What This App Does NOT Do (handled externally):**
- Creating/managing eBay listings (use eBay Seller Hub directly)
- Shipping labels and tracking (use eBay Seller Hub directly)
- Creating invoices (use Wave directly)
- Payment processing (handled by eBay Managed Payments and Wave)

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
- Entities:
  - InventoryItem (eBay inventory with COGS)
  - ComponentItem, ComponentType (computer repair parts)
  - StorageLocation (hierarchical home storage)
  - EbayOrder (synced from eBay API)
  - WaveInvoice, WavePayment (synced from Wave API)
  - BusinessExpense (Schedule C categories)
  - MileageEntry (IRS-compliant mileage log)
- Value Objects (Money, SKU, Address)
- Enums (InventoryStatus, ComponentStatus, RevenueSource, BusinessLine)
- Domain Events (OrderSynced, InventorySold, InvoicePaid)
- Domain Exceptions
- Repository interfaces (IInventoryRepository, IComponentRepository, etc.)
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
- Application service interfaces (IEbayApiClient, IWaveApiClient)
- Validators (FluentValidation)
- Mapping profiles (AutoMapper or manual mappers)

**Key Use Case Categories:**
- Inventory management (eBay items + components)
- Storage location management
- eBay order sync and profit calculation
- Wave invoice/payment sync (read-only)
- Expense and mileage tracking
- Tax reporting (quarterly/annual summaries)

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
- eBay API client implementation (OAuth 2.0, order sync)
- Wave API client implementation (GraphQL, invoice/payment sync)
- Background job services (Hangfire jobs for API sync)
- File storage services (receipt images, inventory photos)

**Rules:**
- Implements interfaces defined in Application/Domain
- Contains all EF Core configuration (fluent API, migrations)
- Handles all external I/O operations
- No business logic (only infrastructure concerns)

### Web Layer (SellerMetrics.Web)
**Purpose:** User interface, controllers, views, and presentation logic

**What belongs here:**
- MVC Controllers (secured with [Authorize])
- Razor Views with Bootstrap 5
- ViewModels (presentation-specific models)
- Microsoft Identity configuration (Entra ID)
- Middleware (security headers, error handling)
- Filters and Action Filters
- Startup/Program configuration
- JavaScript files for client-side interactions

**Rules:**
- Controllers are thin - delegate to Application layer handlers
- All controllers require authentication (Microsoft Identity)
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

### Security Best Practices (Public-Facing Application)
- **NEVER** commit secrets, API keys, or connection strings
- Use user secrets for development, environment variables for production
- Validate and sanitize all user input
- Use parameterized queries (EF Core does this automatically)
- Implement CSRF protection on forms
- Use HTTPS everywhere (Let's Encrypt + HSTS headers)
- Implement proper authentication (Microsoft Identity/Entra ID)
- Follow principle of least privilege for database permissions

**Microsoft Identity (Entra ID) Configuration:**
- Register app in Microsoft Entra ID (Azure Portal)
- Configure single-tenant for personal use
- Use Microsoft.Identity.Web NuGet packages
- All controllers require [Authorize] attribute
- Configure secure cookie settings for production

**Security Headers (Required for public-facing):**
- Strict-Transport-Security (HSTS)
- Content-Security-Policy (CSP)
- X-Content-Type-Options: nosniff
- X-Frame-Options: DENY
- X-XSS-Protection: 1; mode=block
- Referrer-Policy: strict-origin-when-cross-origin

**Rate Limiting:**
- Implement rate limiting middleware to prevent abuse
- Consider IP-based throttling for login attempts

## Sole Proprietorship Tax Compliance

This application is designed to support **sole proprietorship** tax reporting with two revenue streams:

**IRS Schedule C Compliance:**
- Track gross receipts/sales by source:
  - **eBay Reselling** - synced from eBay API
  - **Computer Services** - synced from Wave invoices
- Categorize all business expenses for Schedule C Part II
- Tag expenses by business line (eBay, Services, Shared)
- Generate quarterly and annual profit/loss reports
- Maintain detailed records for 7+ years (IRS audit requirement)

**Mileage Log Requirements (IRS-compliant):**
- Date of trip
- Business purpose (e.g., "Post office - ship eBay orders", "Client visit - Smith", "Thrift store sourcing")
- Starting location and destination
- Miles driven
- Business line (eBay or Computer Services)
- Calculate deduction using current IRS mileage rate (configurable, e.g., $0.67/mile for 2024)
- Support standard mileage method

**Business Expense Categories (Schedule C Part II):**
- Shipping supplies (boxes, tape, bubble wrap, labels) - eBay
- eBay fees (tracked automatically per transaction)
- Office supplies - Shared
- Mileage/vehicle expenses - Both
- Tools & Equipment - Services
- Software/Subscriptions - Shared
- Parts & Materials - Services
- Professional services (accounting, legal) - Shared
- Advertising/marketing - Both
- Other business expenses

**Quarterly Tax Estimates:**
- Calculate estimated tax payments based on quarterly profit (combined both business lines)
- Track quarterly payments made
- Alert when quarterly filing deadlines approach (April 15, June 15, Sept 15, Jan 15)

## Inventory Storage Tracking

**Home Inventory Location (Unified for both business lines):**
- Track physical location of all inventory in home
- Support hierarchical locations: Room > Storage Unit > Bin/Shelf
- Examples: "Garage > Shelf A > Bin 3", "Office > Closet > Top Shelf"
- Quick search/filter to locate items when orders are placed

**eBay Inventory:**
- Track items for resale with COGS
- Status: Unlisted, Listed, Sold
- Link to eBay orders when sold

**Component Inventory (Computer Repair Parts):**
- Track parts available for repairs (RAM, SSDs, HDDs, power supplies, etc.)
- Track quantity for items with multiples
- Status: Available, Reserved, Used, Sold
- Source tracking: Purchased, Salvaged, Customer-provided
- Low stock alerts

## eBay API Integration (Order Sync Only)

**Scope:** Sync orders and fees from eBay. Listings and shipping are managed directly in eBay Seller Hub.

**Authentication:**
- Use OAuth 2.0 for eBay API authentication
- Store refresh tokens securely (environment variables)
- Implement token refresh logic

**Rate Limiting:**
- Implement retry policies with exponential backoff (Polly library)
- Track API call quotas
- Use background jobs (Hangfire) for synchronization

**Error Handling:**
- Handle eBay API errors gracefully (429 Too Many Requests, 401 Unauthorized)
- Log all API errors with request/response details
- Implement circuit breaker pattern for API failures

**Data to Sync from eBay:**
- Order details (order ID, date, buyer info, gross sale price)
- Fees breakdown (final value fee, payment processing fee)
- Shipping cost (what buyer paid)
- Item sold (match to local inventory by SKU or title)

**NOT Synced (use eBay directly):**
- Listing creation/management
- Shipping labels and tracking
- Buyer communication

## Wave API Integration (Read-Only Sync)

**Scope:** Pull invoices and payments from Wave for visibility. Invoice creation is done directly in Wave.

**API Type:** Wave uses a GraphQL API

**Authentication:**
- OAuth 2.0 or API token (depending on Wave's current requirements)
- Store credentials securely (environment variables)

**Rate Limiting:**
- Implement retry policies with exponential backoff (Polly)
- Respect Wave API rate limits

**Error Handling:**
- Handle API errors gracefully
- Log all API errors with request/response details
- Graceful degradation if Wave is unavailable

**Data to Sync from Wave (Read-Only):**
- Invoices (number, customer, date, amount, status)
- Payments (date, amount, method)
- Customer names (for display purposes)

**NOT Synced (use Wave directly):**
- Creating/editing invoices
- Recording payments
- Managing customers
- Bank connections and reconciliation

## Financial Calculations

**Critical Rules:**
- **ALWAYS** use `decimal` type for monetary values (NEVER float or double)
- Round monetary values to 2 decimal places for display
- Store currency code with monetary amounts (use Money value object)
- Track revenue by source (eBay vs Computer Services)

**eBay Profit Calculation:**
```
Gross Sale = Sale Price from eBay
Fees = eBay Final Value Fee + Payment Processing Fee
Shipping Paid = Actual shipping cost you paid (manual entry)
Net Payout = Gross Sale - Fees

Profit = Net Payout - COGS - Shipping Paid
```

**Service Revenue (from Wave):**
```
Service Revenue = Sum of paid Wave invoices
Service Profit = Service Revenue - Related Expenses (optional tracking)
```

**Combined Reporting:**
```
Total Revenue = eBay Net Payout + Service Revenue
Total Expenses = Business Expenses + Mileage Deduction
Net Profit = Total Revenue - Total Expenses - COGS
```

**Tax Reporting:**
- Track all revenue by quarter (Q1 2025, Q2 2025, etc.)
- Separate eBay revenue from Computer Services revenue
- Business expenses must be categorized for Schedule C
- Tag expenses by business line (eBay, Services, Shared)
- Mileage log must include: Date, Purpose, Starting Location, Destination, Miles, Business Line
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

## CI/CD Pipeline (GitHub Actions + Self-Hosted)

**CI Pipeline (on PR):**
- Restore dependencies
- Build solution
- Run all tests
- Check code coverage (fail if below threshold)

**CD Pipeline (on merge to main):**
- Build and publish application
- SSH to VPS/home server
- Deploy application files
- Run database migrations
- Restart systemd service
- Run health check

**Self-Hosted Infrastructure:**
- VPS or home server (Linux recommended)
- SQL Server Express (local) or remote SQL Server
- nginx or Caddy as reverse proxy
- Let's Encrypt for SSL certificates
- systemd for service management

**Secrets Management:**
- Store in GitHub Secrets for CI/CD
- Use environment variables on server (not in config files)
- API credentials: eBay, Wave, Microsoft Identity

## Performance Optimization

- Use caching for frequently accessed, rarely changed data (IMemoryCache or IDistributedCache)
- Implement pagination on all list endpoints (Page size: 20-50 items)
- Use database indexes strategically (avoid over-indexing)
- Profile slow queries with EF Core logging or SQL Profiler
- Use async/await for all I/O operations
- Consider read replicas for reporting queries (if needed at scale)
- Use projection queries (`.Select()`) instead of loading full entities when possible

## Documentation Standards

- Use XML documentation comments for all public APIs
- Keep README.md updated with setup instructions
- Document architectural decisions in ADR (Architecture Decision Records)
- Update TODO.md as tasks are completed
- Inline comments for complex business logic only (code should be self-documenting)
