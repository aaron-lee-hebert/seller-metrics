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

- [x] Create `src/` directory in repository root
- [x] Create .NET 9 solution file: `src/SellerMetrics.sln`
- [x] Create Clean Architecture projects in `src/`:
  - [x] SellerMetrics.Domain (Core domain entities, value objects, interfaces)
  - [x] SellerMetrics.Application (Use cases, DTOs, business logic, CQRS handlers)
  - [x] SellerMetrics.Infrastructure (EF Core, eBay API client, external services)
  - [x] SellerMetrics.Web (ASP.NET Core MVC, Bootstrap 5 UI)
- [x] Create NUnit test project in `src/`:
  - [x] SellerMetrics.Tests (tests for all layers - Domain, Application, Infrastructure)
- [x] Add all projects to solution file
- [x] Configure solution dependencies (Domain ← Application ← Infrastructure/Web)
- [x] Configure test project references (Tests → Domain, Application, Infrastructure, Web)
- [x] Set up Directory.Build.props in `src/` for shared project settings
- [x] Configure .editorconfig in `src/` for code style consistency

### Database & Entity Framework Core

- [x] Set up PostgreSQL database connection
- [x] Configure EF Core with DbContext in Infrastructure layer
- [x] Implement Repository pattern with generic repository base
- [x] Configure database connection string management (user secrets, env variables)
- [ ] Create initial migration for database schema (after domain entities are created)

### Development Environment

- [x] Configure user secrets for sensitive configuration (eBay API keys, connection strings)
- [x] Set up appsettings.json structure (Development, Staging, Production)
- [x] Configure logging providers (Console, Debug) - Sentry.io deferred for later
- [x] Set up development SSL certificate (using default ASP.NET Core dev certificate)

---

## Authentication & Security (ASP.NET Core Identity)

### ASP.NET Core Identity Integration

- [x] Install ASP.NET Core Identity packages (Identity.EntityFrameworkCore, Identity.UI)
- [x] Create ApplicationUser entity extending IdentityUser
- [x] Update DbContext to inherit from IdentityDbContext
- [x] Configure Identity services with password/lockout requirements
- [x] Configure authentication in Program.cs
- [x] Implement [Authorize] on controllers
- [x] Configure secure cookie settings
- [x] Add login/logout UI flow (via Identity.UI default pages)
- [x] Configure Twilio SendGrid for transactional emails
- [ ] Set up authorization policies (deferred until roles needed)

### Security Hardening (Public-Facing)

- [x] Configure HTTPS with Let's Encrypt certificate (handled by reverse proxy)
- [x] Set up HSTS headers (configured in Program.cs for non-development)
- [x] Implement CSRF protection on all forms (antiforgery configured in Program.cs)
- [x] Configure Content Security Policy (CSP) headers (SecurityHeadersMiddleware)
- [x] Add rate limiting middleware (AspNetCoreRateLimit configured)
- [x] Implement request validation and input sanitization (ASP.NET Core model binding)
- [x] Configure CORS if API endpoints needed (not needed - server-rendered MVC only)
- [x] Set up security headers (X-Frame-Options, X-Content-Type-Options, etc. in SecurityHeadersMiddleware)
- [x] Store secrets securely (user secrets for development, environment variables for production)

---

## Inventory Management (eBay + Components)

### Storage Location Tracking

- [x] Create StorageLocation entity
  - [x] Hierarchical structure: Room > Unit > Bin/Shelf (supports arbitrary depth)
  - [x] Examples: "Garage > Shelf A > Bin 3", "Office > Closet > Top Shelf"
  - [x] Support for both eBay inventory and repair components
  - [x] Soft delete with 30-day retention period
- [x] Create StorageLocation use cases:
  - [x] CreateStorageLocation command
  - [x] UpdateStorageLocation command
  - [x] DeleteStorageLocation command (prevent if items exist)
  - [x] GetStorageLocationHierarchy query
  - [x] GetAllStorageLocations query (flat list for dropdowns)
  - [ ] GetItemsByLocation query (deferred until inventory entities complete)

### eBay Inventory

- [x] Create InventoryItem entity
  - [x] InternalSku (auto-generated, format: INV-YYYYMMDD-XXXX)
  - [x] EbaySku (optional - for items with eBay SKU)
  - [x] Title/Description
  - [x] COGS (Money value object with currency support)
  - [x] PurchaseDate
  - [x] StorageLocationId (where it's stored)
  - [x] Status (Unlisted, Listed, Sold) with enum
  - [x] Condition (EbayCondition enum matching eBay's values)
  - [x] Notes
  - [x] PhotoPath (string for future upload feature)
  - [x] Soft delete with 30-day retention period
- [x] Create InventoryItem use cases:
  - [x] CreateInventoryItem command (with SKU auto-generation)
  - [x] UpdateInventoryItem command
  - [x] MoveInventoryItem command (change location)
  - [x] MarkAsSold command
  - [x] DeleteInventoryItem command (soft delete)
  - [x] GetInventoryList query (filter by status, location)
  - [x] GetInventoryItem query (single item details)
  - [x] SearchInventory query (find item by title, SKU, notes)
  - [x] GetInventoryValue query (total COGS of unsold items)

### Component Inventory (Computer Repair Parts)

- [x] Create ComponentType entity (catalog of part types)
  - [x] Name (RAM, SSD, HDD, Power Supply, etc.)
  - [x] DefaultCategory (for expense tracking if purchased)
  - [x] Predefined seed data for common types (13 types)
  - [x] Support for user-created custom types
- [x] Create ComponentItem entity
  - [x] ComponentTypeId
  - [x] Description (e.g., "8GB DDR4 2666MHz", "500GB Samsung 860 EVO")
  - [x] Quantity (track multiples of same component)
  - [x] UnitCost (Money value object with currency)
  - [x] StorageLocationId
  - [x] Status (Available, Reserved, Used, Sold) with enum
  - [x] AcquiredDate
  - [x] Source (Purchased, Salvaged, Customer-provided) with enum
  - [x] Notes
  - [x] ServiceJobId (link to service job for Reserved components)
  - [x] Soft delete with 30-day retention period
- [x] Create ComponentQuantityAdjustment entity (audit trail)
- [x] Create ServiceJob entity (for reserving components)
- [x] Create ComponentItem use cases:
  - [x] CreateComponentItem command
  - [x] UpdateComponentItem command
  - [x] AdjustQuantity command (with audit trail)
  - [x] MoveComponent command (change location)
  - [x] UseComponent command (mark as used in repair)
  - [x] DeleteComponentItem command (soft delete)
  - [x] GetComponentList query (filter by type, location, status)
  - [x] GetLowStockComponents query
  - [x] GetComponentValue query (total value of parts inventory)
  - [x] GetComponentTypes query

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

- [x] Create RevenueSource enum (eBay, ComputerServices)
- [x] Create RevenueEntryType enum (Manual, EbaySynced, WaveSynced)
- [x] Create RevenueEntry entity with support for both synced and manual entries
  - [x] Link to eBay orders via EbayOrderId
  - [x] Link to Wave invoices via WaveInvoiceNumber
  - [x] Link to InventoryItem for eBay sales
  - [x] Link to ServiceJob for service revenue
  - [x] Money value objects for GrossAmount and Fees
- [x] Create FiscalYearConfiguration entity for fiscal year settings
  - [x] Configurable fiscal year start month
  - [x] Methods for calculating fiscal year, quarter, and date ranges
- [x] Create IRevenueEntryRepository with specialized queries
- [x] Create IFiscalYearConfigurationRepository
- [x] Create EF Core configurations for RevenueEntry and FiscalYearConfiguration
- [x] Create revenue commands:
  - [x] CreateRevenueEntry command (with duplicate prevention)
  - [x] UpdateRevenueEntry command
  - [x] DeleteRevenueEntry command (soft delete)
- [x] Create unified revenue queries:
  - [x] GetRevenueEntry query (single entry)
  - [x] GetRevenueList query (filtered list)
  - [x] GetRevenueBySource query (eBay vs Services breakdown)
  - [x] GetMonthlyRevenue query
  - [x] GetQuarterlyRevenue query (fiscal year aware)
  - [x] GetYearToDateRevenue query (fiscal year aware)

### Profit Calculation

- [x] eBay Profit = NetRevenue - COGS (calculated from linked inventory items)
- [x] Service Profit = NetRevenue - Component Costs - Expenses (linked to service jobs)
- [x] Combined profit view for tax reporting
- [x] Create profit DTOs:
  - [x] ProfitBySourceDto
  - [x] CombinedProfitDto
  - [x] MonthlyProfitDto
  - [x] QuarterlyProfitDto
  - [x] ServiceJobProfitDto
  - [x] TaxReportProfitDto (Schedule C format)
- [x] Create profit queries:
  - [x] GetProfitBySource query (eBay vs Services breakdown)
  - [x] GetCombinedProfit query (unified view)
  - [x] GetQuarterlyProfit query (fiscal year aware)
  - [x] GetServiceJobProfit query (per-job profit calculation)
  - [x] GetTaxReportProfit query (Schedule C format with quarterly breakdown)

### Business Expenses

- [x] Create BusinessExpense entity
  - [x] ExpenseDate
  - [x] Description
  - [x] Amount (Money value object)
  - [x] Category (ExpenseCategory enum)
  - [x] BusinessLine (single line: eBay, ComputerServices, Shared)
  - [x] Vendor
  - [x] ReceiptPath (file path for future upload)
  - [x] Notes
  - [x] ServiceJobId (link expenses to service jobs)
  - [x] IsTaxDeductible flag
  - [x] ReferenceNumber
  - [x] Soft delete with 30-day retention
- [x] Create ExpenseCategory enum (IRS Schedule C categories):
  - [x] Shipping Supplies
  - [x] Office Supplies
  - [x] Advertising/Marketing
  - [x] Professional Services
  - [x] Vehicle/Mileage
  - [x] Tools & Equipment
  - [x] Software/Subscriptions
  - [x] Parts & Materials
  - [x] Postage & Shipping
  - [x] Insurance, Interest, Bank Fees
  - [x] Education/Training
  - [x] Utilities, Rent
  - [x] Other
- [x] Create BusinessLine enum (eBay, ComputerServices, Shared)
- [x] Create IBusinessExpenseRepository with specialized queries
- [x] Create EF Core configuration for BusinessExpense
- [x] Create expense commands:
  - [x] CreateExpense command
  - [x] UpdateExpense command
  - [x] DeleteExpense command (soft delete)
- [x] Create expense queries:
  - [x] GetExpense query (single expense)
  - [x] GetExpensesByCategory query
  - [x] GetExpensesByBusinessLine query
  - [x] GetExpensesByDateRange query
  - [x] GetExpenseSummary query (with category and business line breakdown)

### Mileage Log

- [x] Create MileageEntry entity
  - [x] TripDate
  - [x] Purpose (IRS-compliant description)
  - [x] StartLocation
  - [x] Destination
  - [x] Miles (one-way distance)
  - [x] IsRoundTrip flag (doubles miles for deduction)
  - [x] TotalMiles (computed: Miles * 2 if round trip)
  - [x] BusinessLine (eBay, ComputerServices, Shared)
  - [x] Notes
  - [x] ServiceJobId (link to service jobs)
  - [x] OdometerStart/OdometerEnd (optional detailed tracking)
  - [x] Soft delete with 30-day retention
- [x] Create IrsMileageRate entity with historical rates:
  - [x] Year, StandardRate, MedicalRate, CharitableRate
  - [x] EffectiveDate (supports mid-year rate changes)
  - [x] Seed data: 2024 ($0.67), 2025 ($0.70)
- [x] Create IMileageEntryRepository with specialized queries
- [x] Create IIrsMileageRateRepository with date-based rate lookup
- [x] Create EF Core configurations with indexes and seed data
- [x] Create mileage commands:
  - [x] CreateMileageEntry command
  - [x] UpdateMileageEntry command
  - [x] DeleteMileageEntry command (soft delete)
- [x] Create mileage queries:
  - [x] GetMileageEntry query (single entry)
  - [x] GetMileageLog query (filter by date range, business line)
  - [x] CalculateMileageDeduction query (with IRS rate lookup)
  - [x] GetIrsMileageRates query (list all rates)

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

- [ ] Create \_Layout.cshtml with Bootstrap 5
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

## CI/CD & DevOps

### GitHub Actions

- [ ] Create CI workflow for build and test on PR
- [ ] Set up automated testing in pipeline
- [ ] Configure code coverage reporting
- [ ] Implement CD workflow for VPS deployment
- [ ] Add database migration step in deployment pipeline
- [ ] Configure environment-specific deployments (Staging, Production)
- [ ] Set up GitHub Secrets for VPS credentials and connection strings

### Monitoring & Logging

- [ ] Configure Sentry logging
- [ ] Set up custom metrics for eBay API usage
- [ ] Create alerts for application errors
- [ ] Implement health check endpoints
- [ ] Add performance monitoring for database queries

---

## Testing

### Test Project Setup (SellerMetrics.Tests)

- [ ] Set up NUnit test project (src/SellerMetrics.Tests)
- [ ] Add NuGet packages:
  - [ ] NUnit
  - [ ] NUnit3TestAdapter
  - [ ] Microsoft.NET.Test.Sdk
  - [ ] Moq or NSubstitute (for mocking)
  - [ ] FluentAssertions (for readable assertions)
  - [ ] Testcontainers (for integration tests with real database)
- [ ] Create folder structure in test project:
  - [ ] Domain/ (unit tests for domain entities)
  - [ ] Application/ (unit tests for handlers)
  - [ ] Infrastructure/ (integration tests)
  - [ ] Helpers/ (test utilities, builders, fixtures)

### Unit Tests - Domain Layer

- [ ] Write unit tests for domain entities and business rules (SellerMetrics.Tests/Domain/)
  - [ ] Test Inventory entity validation
  - [ ] Test StorageLocation hierarchy
  - [ ] Test Order profit calculation logic
  - [ ] Test Money value object
  - [ ] Test mileage deduction calculations
  - [ ] Test quarterly summary aggregation
  - [ ] Test business expense categorization

### Unit Tests - Application Layer

- [ ] Write unit tests for command/query handlers (SellerMetrics.Tests/Application/)
  - [ ] Test CreateInventoryItem command handler
  - [ ] Test SyncOrdersFromEbay command handler
  - [ ] Test GenerateQuarterlySummary query handler
  - [ ] Test CalculateMileageDeduction query handler
- [ ] Mock external dependencies (IEbayApiClient, repositories)
- [ ] Achieve minimum 80% code coverage for business logic

### Integration Tests - Infrastructure Layer

- [ ] Set up integration tests (SellerMetrics.Tests/Infrastructure/)
- [ ] Configure TestContainers for PostgreSQL or use in-memory database
- [ ] Write integration tests for repositories
  - [ ] Test InventoryRepository CRUD operations
  - [ ] Test OrderRepository with complex queries
  - [ ] Test QuarterlySummary aggregation queries
- [ ] Test EF Core configurations and migrations
- [ ] Test eBay API client integration (with mocked API or test account)
- [ ] Test quarterly summary calculations with real data

### UI Tests (Optional)

- [ ] Consider Playwright or Selenium for critical user flows
- [ ] Test inventory creation workflow
- [ ] Test order sync and display

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
- [ ] Update \_Layout.cshtml to show login/logout links and user info
- [ ] Create Account management area in Web project
  - [ ] Change password functionality
  - [ ] Update profile functionality
  - [ ] Email confirmation workflow (optional)
- [ ] Implement role-based menu navigation in \_Layout.cshtml
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
