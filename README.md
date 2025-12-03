# SellerMetrics

**SellerMetrics** is a comprehensive inventory and financial management system designed specifically for eBay resellers. Track your cost of goods, calculate actual profit margins after fees, monitor monthly performance, and gain actionable insights into your reselling business. Built with modern ASP.NET Core MVC, Entity Framework Core, and SQL Server, SellerMetrics offers a robust, self-hosted alternative to spreadsheet-based tracking systems.

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
- SQL Server
- Bootstrap 5

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or higher edition)

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

   # Set the SQL Server connection string
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SellerMetrics;Trusted_Connection=True;TrustServerCertificate=True;" --project src/SellerMetrics.Web

   # Set eBay API credentials (when ready)
   dotnet user-secrets set "EbayApi:ClientId" "your-client-id" --project src/SellerMetrics.Web
   dotnet user-secrets set "EbayApi:ClientSecret" "your-client-secret" --project src/SellerMetrics.Web

   # List configured secrets
   dotnet user-secrets list --project src/SellerMetrics.Web
   ```

   **SQL Server Connection String Format:**
   ```
   # Windows Authentication (Recommended for Development)
   Server=localhost;Database=SellerMetrics;Trusted_Connection=True;TrustServerCertificate=True;

   # SQL Authentication
   Server=localhost;Database=SellerMetrics;User Id=your_username;Password=your_password;TrustServerCertificate=True;
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

This project follows **Clean Architecture** principles:

```
seller-metrics/
├── src/
│   ├── SellerMetrics.Domain/          # Core entities, value objects, interfaces
│   ├── SellerMetrics.Application/     # Use cases, DTOs, handlers
│   ├── SellerMetrics.Infrastructure/  # EF Core, API clients, external services
│   ├── SellerMetrics.Web/             # MVC controllers, views, UI
│   ├── SellerMetrics.Tests/           # NUnit tests for all layers
│   └── SellerMetrics.sln
├── CLAUDE.md                          # AI assistant guidelines
├── TODO.md                            # Development task tracking
└── README.md
```

## Deployment

SellerMetrics is designed for self-hosting on a VPS or home server.

### Production Requirements

- Linux server (Ubuntu 22.04+ recommended) or Windows Server
- .NET 9 Runtime
- SQL Server (Express or higher)
- Reverse proxy (nginx or Caddy)
- SSL certificate (Let's Encrypt recommended)
- Domain name (or dynamic DNS)

### Basic Deployment Steps

1. **Publish the application:**
   ```bash
   dotnet publish src/SellerMetrics.Web -c Release -o ./publish
   ```

2. **Copy files to server** via SCP, rsync, or your preferred method

3. **Set environment variables** on the server for all secrets

4. **Configure reverse proxy** (nginx/Caddy) with SSL termination

5. **Create systemd service** for automatic startup and restart

6. **Run database migrations:**
   ```bash
   dotnet ef database update --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web
   ```

See [TODO.md](TODO.md) for detailed deployment tasks and CI/CD setup with GitHub Actions.

## Configuration

### Environment Variables (Production)

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `EbayApi__ClientId` | eBay API OAuth client ID |
| `EbayApi__ClientSecret` | eBay API OAuth client secret |
| `WaveApi__AccessToken` | Wave API access token |
| `AzureAd__TenantId` | Microsoft Entra ID tenant |
| `AzureAd__ClientId` | Microsoft Entra ID app client ID |
| `AzureAd__ClientSecret` | Microsoft Entra ID client secret |

### API Setup

#### eBay Developer Account
1. Create an account at [developer.ebay.com](https://developer.ebay.com)
2. Create an application to get API credentials
3. Configure OAuth consent and redirect URIs

#### Wave API
1. Access Wave API settings in your Wave account
2. Generate an API token or configure OAuth

#### Microsoft Entra ID
1. Register an application in [Azure Portal](https://portal.azure.com)
2. Configure redirect URIs for your domain
3. Set supported account types (single tenant recommended)

## Contributing

Contributions are welcome! Please follow these guidelines:

### Development Workflow

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes following the coding standards in [CLAUDE.md](CLAUDE.md)
4. Write tests for new functionality
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

### Coding Standards

- Follow Clean Architecture principles
- Adhere to SOLID principles
- Use async/await for all I/O operations
- Write unit tests for business logic
- Use `decimal` for all monetary values
- See [CLAUDE.md](CLAUDE.md) for detailed guidelines

### Reporting Issues

Please use GitHub Issues to report bugs or request features. Include:
- Clear description of the issue
- Steps to reproduce (for bugs)
- Expected vs actual behavior
- Environment details (.NET version, OS, SQL Server version)

## Roadmap

See [TODO.md](TODO.md) for the full development roadmap. Key upcoming features:

- [ ] Photo upload for inventory items
- [ ] Barcode/QR scanning for inventory lookup
- [ ] Mobile-friendly PWA
- [ ] Receipt OCR for expense entry
- [ ] Parts tracking (link components to service jobs)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) - Web framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [Bootstrap](https://getbootstrap.com/) - UI framework
- [Hangfire](https://www.hangfire.io/) - Background job processing
- [eBay Developers Program](https://developer.ebay.com/) - API integration
- [Wave](https://www.waveapps.com/) - Accounting integration

---

**Note:** This application is designed for personal/small business use. It is not intended to replace professional accounting software or tax advice. Always consult with a tax professional for your specific situation.
