# SellerMetrics - Project TODO

This file tracks all development tasks for the SellerMetrics application.

**Business Context:** This application supports a sole proprietorship with two revenue streams:
1. **eBay Reselling** - Track inventory, COGS, storage locations, profit from synced orders
2. **Computer Support Services** - Track component inventory, view invoices/payments from Wave

**Key Integration Points:**
- **eBay API** - Sync orders and fees (listings/shipping done directly in eBay Seller Hub)
- **Wave API** - Pull invoices and payments for visibility (invoicing done directly in Wave)

**Hosting:** Self-hosted on VPS or home server (public-facing with Microsoft Identity authentication)

**Primary Goals:**
- Know where inventory is stored in my home (eBay items + repair components)
- See combined revenue and profit across both business lines
- Track expenses and mileage for tax reporting (Schedule C)

---

## Project Setup & Infrastructure

### Solution Structure
- [X] Create `src/` directory in repository root
- [X] Create .NET 9 solution file: `src/SellerMetrics.sln`
- [X] Create Clean Architecture projects in `src/`:
  - [X] SellerMetrics.Domain (Core domain entities, value objects, interfaces)
  - [X] SellerMetrics.Application (Use cases, DTOs, business logic, CQRS handlers)
  - [X] SellerMetrics.Infrastructure (EF Core, eBay API client, external services)
  - [X] SellerMetrics.Web (ASP.NET Core MVC, Bootstrap 5 UI)
- [X] Create NUnit test project in `src/`:
  - [X] SellerMetrics.Tests (tests for all layers - Domain, Application, Infrastructure)
- [X] Add all projects to solution file
- [X] Configure solution dependencies (Domain ← Application ← Infrastructure/Web)
- [X] Configure test project references (Tests → Domain, Application, Infrastructure, Web)
- [X] Set up Directory.Build.props in `src/` for shared project settings
- [X] Configure .editorconfig in `src/` for code style consistency

### Database & Entity Framework Core
- [ ] Set up SQL Server connection (local SQL Server or SQL Server Express)
- [ ] Configure EF Core DbContext in Infrastructure layer
- [ ] Implement Repository pattern
- [ ] Configure connection string management (user secrets for dev, environment variables for prod)
- [ ] Create initial migration
- [ ] Set up database seeding (storage locations, expense categories, component types)

### Development Environment
- [ ] Configure user secrets (eBay API, Wave API, connection strings)
- [ ] Set up appsettings.json structure (Development, Production)
- [ ] Configure logging (Console, File, Seq/Serilog optional)

---

## Authentication & Security (Microsoft Identity)

### Microsoft Entra ID (Azure AD) Setup
- [ ] Register application in Microsoft Entra ID (Azure Portal)
- [ ] Configure redirect URIs for your domain
- [ ] Set up client ID and tenant ID
- [ ] Configure supported account types (single tenant for personal use)

### ASP.NET Core Identity Integration
- [ ] Install Microsoft.Identity.Web packages
- [ ] Configure authentication in Program.cs
- [ ] Set up authorization policies
- [ ] Implement [Authorize] on controllers
- [ ] Configure secure cookie settings
- [ ] Add login/logout UI flow

### Security Hardening (Public-Facing)
- [ ] Configure HTTPS with Let's Encrypt certificate
- [ ] Set up HSTS headers
- [ ] Implement CSRF protection on all forms
- [ ] Configure Content Security Policy (CSP) headers
- [ ] Add rate limiting middleware
- [ ] Implement request validation and input sanitization
- [ ] Configure CORS if API endpoints needed
- [ ] Set up security headers (X-Frame-Options, X-Content-Type-Options, etc.)
- [ ] Store secrets securely (environment variables, not in config files)

---

## Inventory Management (eBay + Components)

### Storage Location Tracking
- [ ] Create StorageLocation entity
  - [ ] Hierarchical structure: Room > Unit > Bin/Shelf
  - [ ] Examples: "Garage > Shelf A > Bin 3", "Office > Closet > Top Shelf"
  - [ ] Support for both eBay inventory and repair components
- [ ] Create StorageLocation use cases:
  - [ ] CreateStorageLocation command
  - [ ] UpdateStorageLocation command
  - [ ] DeleteStorageLocation command (prevent if items exist)
  - [ ] GetStorageLocationHierarchy query
  - [ ] GetItemsByLocation query

### eBay Inventory
- [ ] Create InventoryItem entity
  - [ ] SKU (optional - for items with eBay SKU)
  - [ ] Title/Description
  - [ ] COGS (cost of goods sold)
  - [ ] PurchaseDate
  - [ ] StorageLocationId (where it's stored)
  - [ ] Status (Unlisted, Listed, Sold)
  - [ ] Condition
  - [ ] Notes
  - [ ] PhotoPath (optional)
- [ ] Create InventoryItem use cases:
  - [ ] CreateInventoryItem command
  - [ ] UpdateInventoryItem command
  - [ ] MoveInventoryItem command (change location)
  - [ ] MarkAsSold command (link to order when synced)
  - [ ] GetInventoryList query (filter by status, location)
  - [ ] GetInventoryDetails query
  - [ ] SearchInventory query (find item by title, SKU, location)
  - [ ] GetInventoryValue query (total COGS of unsold items)

### Component Inventory (Computer Repair Parts)
- [ ] Create ComponentType entity (catalog of part types)
  - [ ] Name (RAM, SSD, HDD, Power Supply, etc.)
  - [ ] DefaultCategory (for expense tracking if purchased)
- [ ] Create ComponentItem entity
  - [ ] ComponentTypeId
  - [ ] Description (e.g., "8GB DDR4 2666MHz", "500GB Samsung 860 EVO")
  - [ ] Quantity (track multiples of same component)
  - [ ] UnitCost
  - [ ] StorageLocationId
  - [ ] Status (Available, Reserved, Used, Sold)
  - [ ] AcquiredDate
  - [ ] Source (Purchased, Salvaged, Customer-provided)
  - [ ] Notes
- [ ] Create ComponentItem use cases:
  - [ ] CreateComponentItem command
  - [ ] UpdateComponentItem command
  - [ ] AdjustQuantity command (add/remove stock)
  - [ ] MoveComponent command (change location)
  - [ ] UseComponent command (mark as used in repair)
  - [ ] GetComponentList query (filter by type, location, status)
  - [ ] GetLowStockComponents query
  - [ ] GetComponentValue query (total value of parts inventory)

---

## eBay Integration (Order Sync)

### eBay API Client
- [ ] Create IEbayApiClient interface
- [ ] Implement OAuth 2.0 authentication
- [ ] Implement GetOrders API call (sync sold items)
- [ ] Configure rate limiting and retry policies (Polly)
- [ ] Handle API errors gracefully

### Order Sync
- [ ] Create EbayOrder entity
  - [ ] EbayOrderId
  - [ ] OrderDate
  - [ ] BuyerUsername
  - [ ] GrossSale (sale price)
  - [ ] EbayFees (final value fee + payment processing)
  - [ ] ShippingCost (what buyer paid)
  - [ ] ShippingActual (what you paid - manual entry)
  - [ ] InventoryItemId (link to local inventory)
  - [ ] NetPayout (Gross - Fees)
  - [ ] Profit (NetPayout - COGS - ShippingActual)
- [ ] Create order sync use cases:
  - [ ] SyncOrdersFromEbay command (background job)
  - [ ] LinkOrderToInventory command (match order to inventory item)
  - [ ] UpdateShippingCost command (enter actual shipping paid)
  - [ ] GetOrderList query (with profit calculations)
  - [ ] GetOrderDetails query

---

## Wave Integration (Read-Only Sync)

### Wave API Client
- [ ] Research Wave API (GraphQL-based)
- [ ] Create IWaveApiClient interface
- [ ] Implement authentication (OAuth or API token)
- [ ] Handle API errors and rate limiting

### Invoice/Payment Sync (Read-Only)
- [ ] Create WaveInvoice entity (local cache of Wave data)
  - [ ] WaveInvoiceId
  - [ ] InvoiceNumber
  - [ ] CustomerName
  - [ ] InvoiceDate
  - [ ] DueDate
  - [ ] TotalAmount
  - [ ] AmountPaid
  - [ ] Status (Draft, Sent, Viewed, Paid, Overdue)
  - [ ] LastSyncedAt
- [ ] Create WavePayment entity
  - [ ] WavePaymentId
  - [ ] WaveInvoiceId
  - [ ] PaymentDate
  - [ ] Amount
  - [ ] PaymentMethod
- [ ] Create Wave sync use cases:
  - [ ] SyncInvoicesFromWave command (background job)
  - [ ] SyncPaymentsFromWave command
  - [ ] GetInvoiceList query (view-only)
  - [ ] GetInvoiceDetails query
  - [ ] GetUnpaidInvoices query
  - [ ] GetServiceRevenue query (sum of paid invoices by period)

---

## Financial Tracking

### Revenue Tracking
- [ ] Create RevenueSource enum (eBay, ComputerServices)
- [ ] Aggregate revenue from:
  - [ ] eBay: Sum of NetPayout from synced orders
  - [ ] Services: Sum of paid Wave invoices
- [ ] Create unified revenue queries:
  - [ ] GetRevenueBySource query (eBay vs Services breakdown)
  - [ ] GetMonthlyRevenue query
  - [ ] GetQuarterlyRevenue query
  - [ ] GetYearToDateRevenue query

### Profit Calculation
- [ ] eBay Profit = NetPayout - COGS - ActualShipping
- [ ] Service Profit = Invoice Amount - Related Expenses (optional tracking)
- [ ] Combined profit view for tax reporting

### Business Expenses
- [ ] Create BusinessExpense entity
  - [ ] Date
  - [ ] Description
  - [ ] Amount
  - [ ] Category (IRS Schedule C categories)
  - [ ] BusinessLine (eBay, Services, Shared)
  - [ ] ReceiptPath (optional photo)
  - [ ] Notes
- [ ] IRS Schedule C expense categories:
  - [ ] Shipping Supplies
  - [ ] Office Supplies
  - [ ] Advertising/Marketing
  - [ ] Professional Services
  - [ ] Vehicle/Mileage
  - [ ] Tools & Equipment
  - [ ] Software/Subscriptions
  - [ ] Parts & Materials
  - [ ] Other
- [ ] Create expense use cases:
  - [ ] CreateExpense command
  - [ ] UpdateExpense command
  - [ ] DeleteExpense command
  - [ ] GetExpensesByCategory query
  - [ ] GetExpensesByBusinessLine query
  - [ ] GetExpensesByDateRange query

### Mileage Log
- [ ] Create MileageEntry entity
  - [ ] Date
  - [ ] Purpose (e.g., "Post office - ship orders", "Client visit - Smith residence")
  - [ ] StartLocation
  - [ ] Destination
  - [ ] Miles
  - [ ] BusinessLine (eBay, Services)
  - [ ] Notes
- [ ] Store current IRS mileage rate (configurable)
- [ ] Create mileage use cases:
  - [ ] CreateMileageEntry command
  - [ ] UpdateMileageEntry command
  - [ ] DeleteMileageEntry command
  - [ ] GetMileageLog query (by date range, business line)
  - [ ] CalculateMileageDeduction query

---

## Tax Reporting

### Quarterly Summary
- [ ] Create QuarterlySummary view/report
  - [ ] Quarter identifier (Q1 2025, Q2 2025, etc.)
  - [ ] Revenue by source (eBay, Services)
  - [ ] Total Revenue
  - [ ] Expenses by category
  - [ ] Total Expenses
  - [ ] Mileage deduction
  - [ ] Net Profit
- [ ] Create GetQuarterlySummary query

### Annual Summary
- [ ] Create AnnualSummary view/report
  - [ ] Year
  - [ ] Quarterly breakdown
  - [ ] Annual totals
  - [ ] Schedule C preview (categorized for tax filing)
- [ ] Create GetAnnualSummary query
- [ ] Create ExportTaxReport command (CSV/Excel for accountant)

---

## Web Layer (MVC + Bootstrap 5)

### Layout & Navigation
- [ ] Create _Layout.cshtml with Bootstrap 5
- [ ] Implement responsive sidebar navigation:
  - [ ] Dashboard
  - [ ] Inventory (eBay Items)
  - [ ] Components (Repair Parts)
  - [ ] eBay Orders
  - [ ] Service Invoices (Wave)
  - [ ] Expenses
  - [ ] Mileage Log
  - [ ] Reports
  - [ ] Settings
- [ ] Mobile-responsive design
- [ ] Add user info and logout in header (Microsoft Identity)

### Dashboard
- [ ] Key metrics cards:
  - [ ] eBay Inventory Value (total COGS of unsold items)
  - [ ] Component Inventory Value
  - [ ] Quarter-to-Date Revenue (eBay + Services)
  - [ ] Quarter-to-Date Profit
  - [ ] Year-to-Date Revenue
  - [ ] Year-to-Date Profit
- [ ] Recent activity:
  - [ ] Last 5 eBay orders
  - [ ] Last 5 Service invoices (from Wave)
- [ ] Alerts:
  - [ ] Unpaid invoices count
  - [ ] Next quarterly tax deadline
- [ ] Simple chart: Monthly revenue trend (eBay vs Services)

### Inventory UI (eBay Items)
- [ ] List view with DataTables (search, filter, sort)
- [ ] Filter by: Status, Location
- [ ] Add/Edit inventory form
  - [ ] Storage location dropdown (hierarchical)
- [ ] Quick "Find Item" search (for when order comes in)
- [ ] Inventory detail view
- [ ] Status badges (Unlisted, Listed, Sold)

### Components UI (Repair Parts)
- [ ] List view with DataTables
- [ ] Filter by: Type, Location, Status
- [ ] Add/Edit component form
- [ ] Quantity adjustment (+/-)
- [ ] Low stock alert indicator
- [ ] Component detail view

### eBay Orders UI
- [ ] List view with profit calculations
- [ ] Filter by date range
- [ ] Order detail view:
  - [ ] Gross sale, fees, net payout
  - [ ] Link to inventory item (COGS)
  - [ ] Actual shipping cost entry
  - [ ] Calculated profit
- [ ] Manual "Sync Now" button

### Service Invoices UI (Wave Data)
- [ ] List view (read-only from Wave)
- [ ] Filter by status, date range
- [ ] Invoice detail view
- [ ] Payment history
- [ ] Manual "Sync Now" button
- [ ] Link to Wave for editing (external link)

### Expenses UI
- [ ] List view with category grouping
- [ ] Add/Edit expense form
  - [ ] Category dropdown
  - [ ] Business line selector
- [ ] Filter by category, business line, date range
- [ ] Summary totals by category

### Mileage Log UI
- [ ] List view with running total
- [ ] Add/Edit mileage entry form
- [ ] Filter by business line, date range
- [ ] Display current IRS rate
- [ ] Show calculated deduction total

### Reports UI
- [ ] Quarterly summary page
  - [ ] Select quarter
  - [ ] Revenue breakdown (eBay vs Services)
  - [ ] Expense breakdown by category
  - [ ] Mileage deduction
  - [ ] Net profit
- [ ] Annual summary page
  - [ ] Full year view
  - [ ] Schedule C preview format
- [ ] Export to CSV/Excel button

### Storage Locations UI
- [ ] Tree view of locations
- [ ] Add/Edit location form
- [ ] View items at location

---

## Background Jobs

- [ ] Set up Hangfire (with SQL Server storage)
- [ ] eBay order sync job (configurable schedule)
- [ ] Wave invoice/payment sync job (configurable schedule)
- [ ] Hangfire dashboard (secured with Microsoft Identity)

---

## Deployment (Self-Hosted VPS/Home Server)

### Server Setup
- [ ] Provision VPS or configure home server
- [ ] Install .NET 9 runtime
- [ ] Install SQL Server Express or configure remote SQL Server
- [ ] Configure firewall rules (80, 443)
- [ ] Set up reverse proxy (nginx or Caddy recommended)

### SSL/TLS Configuration
- [ ] Register domain name (or use dynamic DNS for home server)
- [ ] Install Certbot for Let's Encrypt
- [ ] Configure automatic certificate renewal
- [ ] Set up HTTPS redirect

### Application Deployment
- [ ] Create systemd service file for ASP.NET Core app
- [ ] Configure environment variables for production
  - [ ] Connection strings
  - [ ] eBay API credentials
  - [ ] Wave API credentials
  - [ ] Microsoft Identity settings
- [ ] Set up log rotation
- [ ] Configure Kestrel for production

### Database Deployment
- [ ] Run EF Core migrations on production database
- [ ] Set up database backup schedule
- [ ] Configure backup retention policy

### Monitoring & Maintenance
- [ ] Set up health check endpoint
- [ ] Configure uptime monitoring (UptimeRobot, Healthchecks.io)
- [ ] Set up log aggregation (optional: Seq, Loki)
- [ ] Configure alerting for errors
- [ ] Document manual backup/restore procedures

### CI/CD (GitHub Actions)
- [ ] Create CI workflow (build, test on PR)
- [ ] Create CD workflow for deployment:
  - [ ] Build and publish application
  - [ ] SSH to server and deploy
  - [ ] Run database migrations
  - [ ] Restart service
- [ ] Store deployment SSH key in GitHub Secrets
- [ ] Store production secrets in GitHub Secrets

### Security

#### Identity & Authentication (Microsoft Identity)
- [ ] Add Microsoft.AspNetCore.Identity.EntityFrameworkCore package to Infrastructure project
- [ ] Add Microsoft.AspNetCore.Identity.UI package to Web project
- [ ] Create ApplicationUser entity inheriting from IdentityUser in Domain layer
  - [ ] Add custom properties (FirstName, LastName, DateCreated, IsActive)
- [ ] Create ApplicationRole entity inheriting from IdentityRole in Domain layer
- [ ] Update Infrastructure DbContext to inherit from IdentityDbContext<ApplicationUser, ApplicationRole, string>
- [ ] Configure Identity in DbContext OnModelCreating (table names, schema, constraints)
- [ ] Configure Identity services in Web Program.cs
  - [ ] Add Identity with ApplicationUser and ApplicationRole
  - [ ] Configure password requirements (length, complexity)
  - [ ] Configure lockout settings (max attempts, duration)
  - [ ] Configure user settings (require unique email, require confirmed email)
  - [ ] Configure cookie settings (expiration, sliding expiration, login path)
- [ ] Scaffold Identity UI pages to Web project for customization
  - [ ] Register page with FirstName and LastName fields
  - [ ] Login page with "Remember Me" option
  - [ ] Forgot Password page
  - [ ] Reset Password page
  - [ ] Two-Factor Authentication pages (optional)
  - [ ] User Profile page with custom fields
- [ ] Create initial EF Core migration for Identity tables
- [ ] Apply migration to create Identity tables in database
- [ ] Create database seed for default admin user and roles
  - [ ] Create "Admin" role
  - [ ] Create "User" role
  - [ ] Create default admin user account
- [ ] Add [Authorize] attributes to protected controllers/actions
- [ ] Add authorization policies for sensitive operations
  - [ ] Policy for Admin-only actions (user management, system settings)
  - [ ] Policy for financial data access (reports, expenses)
  - [ ] Policy for inventory management
- [ ] Update _Layout.cshtml to show login/logout links and user info
- [ ] Create Account management area in Web project
  - [ ] Change password functionality
  - [ ] Update profile functionality
  - [ ] Email confirmation workflow (optional)
- [ ] Implement role-based menu navigation in _Layout.cshtml
- [ ] Add user audit fields to domain entities (CreatedBy, UpdatedBy)
- [ ] Implement ICurrentUserService in Infrastructure to get current user ID
- [ ] Update DbContext SaveChanges to automatically populate audit fields

#### Additional Security Measures
- [ ] Secure eBay API credentials (Key Vault, user secrets)
- [ ] Implement CSRF protection on forms (enabled by default in ASP.NET Core)
- [ ] Add input sanitization and XSS prevention
- [ ] Configure HTTPS redirection and HSTS
- [ ] Implement Content Security Policy (CSP) headers
- [ ] Add rate limiting for API endpoints
- [ ] Configure secure cookie settings (HttpOnly, Secure, SameSite)

## Testing

### Test Project Setup
- [ ] Set up NUnit test project
- [ ] Add packages: NUnit, Moq/NSubstitute, FluentAssertions
- [ ] Create folder structure (Domain/, Application/, Infrastructure/)

### Unit Tests
- [ ] Domain entity tests (profit calculations, validations)
- [ ] Handler tests with mocked dependencies

### Integration Tests
- [ ] Repository tests with in-memory database
- [ ] API client tests with mocked HTTP responses

---

## Future Enhancements (Backlog)

- [ ] Photo upload for inventory items
- [ ] Barcode/QR scanning for inventory lookup
- [ ] Mobile-friendly PWA for quick lookups
- [ ] Email alerts for overdue invoices
- [ ] Recurring expense templates
- [ ] Import expenses from bank CSV
- [ ] Receipt OCR for expense entry
- [ ] Parts used tracking (link component to service job)
- [ ] Multi-year comparison reports
- [ ] 1099-K reconciliation helper
- [ ] Docker containerization for easier deployment
- [ ] Terraform/Ansible scripts for infrastructure as code
