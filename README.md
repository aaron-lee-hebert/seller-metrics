# SellerMetrics

A unified inventory and financial tracking system for sole proprietors managing multiple revenue streams. Built for eBay resellers who also provide computer repair services, SellerMetrics consolidates inventory tracking, profit calculations, and tax reporting into a single self-hosted application.

## Overview

SellerMetrics helps you:
- **Track inventory** - Know where every item is stored in your home (eBay items and repair components)
- **See combined profits** - View revenue and profit from both eBay sales and service invoices in one dashboard
- **Prepare for taxes** - Generate Schedule C reports with categorized expenses and mileage logs

### What This App Does

| Feature | Description |
|---------|-------------|
| eBay Order Sync | Automatically pull orders and fees from eBay API |
| Wave Invoice Sync | Pull invoices and payments from Wave for visibility |
| Inventory Tracking | Track eBay items with COGS and storage locations |
| Component Inventory | Track computer repair parts (RAM, SSDs, etc.) |
| Expense Tracking | Categorize expenses for Schedule C reporting |
| Mileage Log | IRS-compliant mileage tracking for both business lines |
| Tax Reports | Quarterly and annual summaries for tax filing |

### What This App Does NOT Do

- **eBay listings** - Use eBay Seller Hub directly
- **Shipping labels** - Use eBay Seller Hub directly
- **Create invoices** - Use Wave directly
- **Process payments** - Handled by eBay Managed Payments and Wave

## Tech Stack

- **.NET 9** - Runtime and SDK
- **ASP.NET Core MVC** - Web framework
- **Entity Framework Core** - ORM and data access
- **SQL Server** - Database (Express edition works fine)
- **Bootstrap 5** - UI framework
- **Hangfire** - Background job processing
- **Microsoft Identity (Entra ID)** - Authentication

### External Integrations

- **eBay API** - Order and fee synchronization (OAuth 2.0)
- **Wave API** - Invoice and payment visibility (GraphQL)

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or full edition)
- [Git](https://git-scm.com/)
- A Microsoft Entra ID (Azure AD) tenant for authentication

### Clone the Repository

```bash
git clone https://github.com/yourusername/seller-metrics.git
cd seller-metrics
```

### Configure User Secrets (Development)

```bash
# Initialize user secrets
dotnet user-secrets init --project src/SellerMetrics.Web

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=SellerMetrics;Trusted_Connection=True;TrustServerCertificate=True;" --project src/SellerMetrics.Web

# Set eBay API credentials
dotnet user-secrets set "EbayApi:ClientId" "your-ebay-client-id" --project src/SellerMetrics.Web
dotnet user-secrets set "EbayApi:ClientSecret" "your-ebay-client-secret" --project src/SellerMetrics.Web

# Set Wave API credentials
dotnet user-secrets set "WaveApi:AccessToken" "your-wave-access-token" --project src/SellerMetrics.Web

# Set Microsoft Identity settings
dotnet user-secrets set "AzureAd:TenantId" "your-tenant-id" --project src/SellerMetrics.Web
dotnet user-secrets set "AzureAd:ClientId" "your-client-id" --project src/SellerMetrics.Web
```

### Build and Run

```bash
# Restore dependencies and build
dotnet build src/SellerMetrics.sln

# Run database migrations
dotnet ef database update --project src/SellerMetrics.Infrastructure --startup-project src/SellerMetrics.Web

# Run the application
dotnet run --project src/SellerMetrics.Web
```

The application will be available at `https://localhost:5001` (or the port configured in launchSettings.json).

### Running Tests

```bash
# Run all tests
dotnet test src/SellerMetrics.sln

# Run with coverage
dotnet test src/SellerMetrics.sln /p:CollectCoverage=true
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

```
MIT License

Copyright (c) 2025 [Your Name]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## Acknowledgments

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) - Web framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [Bootstrap](https://getbootstrap.com/) - UI framework
- [Hangfire](https://www.hangfire.io/) - Background job processing
- [eBay Developers Program](https://developer.ebay.com/) - API integration
- [Wave](https://www.waveapps.com/) - Accounting integration

---

**Note:** This application is designed for personal/small business use. It is not intended to replace professional accounting software or tax advice. Always consult with a tax professional for your specific situation.
