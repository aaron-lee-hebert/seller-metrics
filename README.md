# SellerMetrics

**SellerMetrics** is a comprehensive inventory and financial management system designed specifically for eBay resellers. Track your cost of goods, calculate actual profit margins after fees, monitor monthly performance, and gain actionable insights into your reselling business. Built with modern ASP.NET Core MVC, Entity Framework Core, and PostgreSQL, SellerMetrics offers a robust, self-hosted alternative to spreadsheet-based tracking systems.

## Key Features

- Inventory management with COGS tracking
- Automated eBay order and listing synchronization
- Real-time profit calculations including eBay fees
- Quarterly and annual financial summaries and reports
- Business expense tracking for non-inventory costs
- Mileage log for business travel (IRS-compliant)
- Responsive dashboard with key business metrics

## Technology Stack

- .NET 9
- ASP.NET Core MVC
- Entity Framework Core
- PostgreSQL
- Bootstrap 5

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (local or remote instance)

### Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/aaron-lee-hebert/seller-metrics.git
   cd seller-metrics
   ```

2. **Configure User Secrets (Development):**

   User secrets store sensitive configuration outside of source control. Initialize and configure them:

   ```bash
   # Initialize user secrets (already configured in project)
   dotnet user-secrets init --project src/SellerMetrics.Web

   # Set the PostgreSQL connection string
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=sellermetrics;Username=your_username;Password=your_password" --project src/SellerMetrics.Web

   # Set eBay API credentials (when ready)
   dotnet user-secrets set "EbayApi:ClientId" "your-client-id" --project src/SellerMetrics.Web
   dotnet user-secrets set "EbayApi:ClientSecret" "your-client-secret" --project src/SellerMetrics.Web

   # List configured secrets
   dotnet user-secrets list --project src/SellerMetrics.Web
   ```

   **PostgreSQL Connection String Format:**
   ```
   Host=localhost;Port=5432;Database=sellermetrics;Username=your_username;Password=your_password
   ```

3. **Build the solution:**
   ```bash
   dotnet build src/SellerMetrics.sln
   ```

4. **Run database migrations** (after entities are created):
   ```bash
   dotnet ef database update --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
   ```

5. **Run the application:**
   ```bash
   dotnet run --project src/SellerMetrics.Web
   ```

   The application will be available at `https://localhost:5001` (or the port configured in launchSettings.json).

### Development Commands

```bash
# Build the solution
dotnet build src/SellerMetrics.sln

# Run with hot reload
dotnet watch --project src/SellerMetrics.Web

# Run tests
dotnet test src/SellerMetrics.sln

# Add a new migration
dotnet ef migrations add <MigrationName> --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web

# Update database
dotnet ef database update --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
```

## Project Structure

This project follows Clean Architecture principles:

```
seller-metrics/
├── src/
│   ├── SellerMetrics.Domain/          # Core business entities and interfaces
│   ├── SellerMetrics.Application/     # Use cases, DTOs, CQRS handlers
│   ├── SellerMetrics.Infrastructure/  # EF Core, external services
│   ├── SellerMetrics.Web/             # ASP.NET Core MVC UI
│   ├── SellerMetrics.Tests/           # Unit and integration tests
│   └── SellerMetrics.sln
├── CLAUDE.md                          # AI assistant guidelines
├── TODO.md                            # Development task tracking
└── README.md
```

## License

This project is private and not licensed for public use
