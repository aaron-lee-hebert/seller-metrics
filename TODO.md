# SellerMetrics - Project TODO

This file tracks all development tasks for the SellerMetrics application.

---

## Project Setup & Infrastructure

### Solution Structure
- [ ] Create `src/` directory in repository root
- [ ] Create .NET 9 solution file: `src/SellerMetrics.sln`
- [ ] Create Clean Architecture projects in `src/`:
  - [ ] SellerMetrics.Domain (Core domain entities, value objects, interfaces)
  - [ ] SellerMetrics.Application (Use cases, DTOs, business logic, CQRS handlers)
  - [ ] SellerMetrics.Infrastructure (EF Core, eBay API client, external services)
  - [ ] SellerMetrics.Web (ASP.NET Core MVC, Bootstrap 5 UI)
- [ ] Create NUnit test project in `src/`:
  - [ ] SellerMetrics.Tests (tests for all layers - Domain, Application, Infrastructure)
- [ ] Add all projects to solution file
- [ ] Configure solution dependencies (Domain ← Application ← Infrastructure/Web)
- [ ] Configure test project references (Tests → Domain, Application, Infrastructure, Web)
- [ ] Set up Directory.Build.props in `src/` for shared project settings
- [ ] Configure .editorconfig in `src/` for code style consistency

### Database & Entity Framework Core
- [ ] Set up SQL Server/Azure SQL Database connection
- [ ] Configure EF Core with DbContext in Infrastructure layer
- [ ] Implement Repository pattern with generic repository base
- [ ] Configure database connection string management (user secrets, env variables)
- [ ] Create initial migration for database schema
- [ ] Set up database seeding for development data (sample inventory, locations, expense categories)

### Development Environment
- [ ] Configure user secrets for sensitive configuration (eBay API keys, connection strings)
- [ ] Set up appsettings.json structure (Development, Staging, Production)
- [ ] Configure logging providers (Console, Debug, Application Insights)
- [ ] Set up development SSL certificate

---

## Core Domain Models

### Inventory Management
- [ ] Create Inventory entity with COGS tracking
- [ ] Implement InventoryItem value objects (SKU, Condition)
- [ ] Create StorageLocation entity (hierarchical: Room > Unit > Shelf/Bin)
- [ ] Add StorageLocationId to Inventory for physical location tracking
- [ ] Define inventory status enumeration (Unlisted, Listed, Sold, Reserved)
- [ ] Create domain events for inventory state changes (ItemAdded, ItemListed, ItemSold)
- [ ] Implement business rules for inventory validation (SKU uniqueness, positive COGS)

### Orders & Sales
- [ ] Create Order entity with eBay order mapping
- [ ] Define OrderItem for line-level details
- [ ] Implement order status workflow (Pending, Paid, Shipped, Completed)
- [ ] Create domain logic for profit calculation (revenue - COGS - fees)
- [ ] Define order domain events (OrderPlaced, OrderShipped, OrderCompleted)

### eBay Listings
- [ ] Create Listing entity synchronized with eBay
- [ ] Define listing status (Draft, Active, Ended, Sold)
- [ ] Link listings to inventory items
- [ ] Implement listing fee tracking
- [ ] Create business rules for listing validation

### Financial Tracking
- [ ] Create BusinessExpense entity for non-inventory costs
- [ ] Define IRS Schedule C expense categories:
  - [ ] Shipping Supplies
  - [ ] Office Supplies
  - [ ] Advertising/Marketing
  - [ ] Professional Services
  - [ ] Storage/Warehouse
  - [ ] Other Business Expenses
- [ ] Create MileageLog entity (Date, Purpose, StartLocation, Destination, Miles, Rate)
- [ ] Create QuarterlySummary aggregate for tax reporting (Q3 2025, Q4 2025, etc.)
- [ ] Create AnnualSummary for yearly tax reports
- [ ] Implement fee calculation service (eBay final value, payment processing)
- [ ] Define value objects for Money type (Currency, Amount)
- [ ] Create TaxableIncome calculation service (matching spreadsheet formula)
- [ ] Implement estimated quarterly tax calculation logic

---

## Application Layer (Use Cases)

### Inventory Use Cases
- [ ] CreateInventoryItem command and handler (with storage location)
- [ ] UpdateInventoryItem command and handler
- [ ] UpdateInventoryLocation command (move item to new location)
- [ ] DeleteInventoryItem command and handler
- [ ] GetInventoryList query and handler (with pagination, filtering by status, location)
- [ ] GetInventoryDetails query and handler
- [ ] SearchInventoryByLocation query (find items in specific storage location)
- [ ] CalculateInventoryValue query (total COGS of unsold inventory)
- [ ] GetUnlistedInventory query (items not yet on eBay)

### Order Use Cases
- [ ] SyncOrdersFromEbay command (background job)
- [ ] GetOrderList query with filters (date range, status)
- [ ] GetOrderDetails query with profit breakdown
- [ ] UpdateOrderStatus command
- [ ] GenerateOrderReport query

### Listing Use Cases
- [ ] SyncListingsFromEbay command
- [ ] CreateListing command (with eBay API push)
- [ ] UpdateListing command
- [ ] EndListing command
- [ ] GetActiveListings query

### Reporting Use Cases
- [ ] GenerateQuarterlySummary query (Q3 2025, Q4 2025 format matching spreadsheet)
- [ ] GenerateAnnualSummary query (for Schedule C tax filing)
- [ ] GetProfitMarginsByCategory query
- [ ] GetTopSellingItems query
- [ ] GetExpenseReport query (by category, date range, for Schedule C)
- [ ] CalculateYearToDateMetrics query
- [ ] ExportTaxReport command (CSV/Excel for accountant)

### Business Expense Use Cases
- [ ] CreateBusinessExpense command and handler
- [ ] UpdateBusinessExpense command and handler
- [ ] DeleteBusinessExpense command and handler
- [ ] GetExpensesByCategory query (for Schedule C reporting)
- [ ] GetExpensesByDateRange query
- [ ] ImportExpenses command (CSV upload)

### Mileage Log Use Cases
- [ ] CreateMileageEntry command and handler
- [ ] UpdateMileageEntry command and handler
- [ ] DeleteMileageEntry command and handler
- [ ] GetMileageLogByDateRange query
- [ ] CalculateMileageDeduction query (using current IRS rate)
- [ ] GetQuarterlyMileageTotal query
- [ ] ExportMileageLog command (for tax records)

### Storage Location Use Cases
- [ ] CreateStorageLocation command (Room, Unit, Shelf)
- [ ] UpdateStorageLocation command
- [ ] DeleteStorageLocation command (if empty)
- [ ] GetStorageLocationHierarchy query (tree view)
- [ ] GetInventoryByLocation query
- [ ] GetStorageUtilization query (items per location)

---

## Infrastructure Layer

### eBay API Integration
- [ ] Create IEbayApiClient interface in Application layer
- [ ] Implement EbayApiClient with OAuth 2.0 authentication
- [ ] Implement GetOrders API call with pagination
- [ ] Implement GetListings API call
- [ ] Implement CreateListing API call
- [ ] Implement UpdateListing API call
- [ ] Configure rate limiting and retry policies (Polly)
- [ ] Add comprehensive error handling and logging
- [ ] Create eBay API response DTOs
- [ ] Implement mapping from eBay DTOs to domain entities

### Data Persistence
- [ ] Configure EF Core DbContext with entity configurations
- [ ] Implement entity type configurations for all domain entities
- [ ] Create repositories implementing domain interfaces
- [ ] Configure indexes for performance (SKU, Order dates, etc.)
- [ ] Implement Unit of Work pattern
- [ ] Add soft delete support for key entities
- [ ] Configure audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)

### Background Jobs
- [ ] Set up Hangfire or similar for background job processing
- [ ] Implement eBay order sync job (hourly/daily)
- [ ] Implement eBay listing sync job
- [ ] Create monthly summary generation job
- [ ] Add job monitoring and failure notifications

---

## Web Layer (MVC + Bootstrap 5)

### Layout & Navigation
- [ ] Create _Layout.cshtml with Bootstrap 5
- [ ] Implement responsive navigation with sidebar
- [ ] Create footer with app version and links
- [ ] Set up Bootstrap 5 theme/customization
- [ ] Implement mobile-responsive hamburger menu
- [ ] Add breadcrumb navigation

### Dashboard
- [ ] Create dashboard view with key metrics cards
- [ ] Display total inventory value (unsold items)
- [ ] Show current quarter revenue, expenses, profit (matching spreadsheet)
- [ ] Display year-to-date totals
- [ ] Add recent orders list (last 10)
- [ ] Create active listings summary
- [ ] Show upcoming quarterly tax deadline
- [ ] Implement charts for profit trends by quarter (Chart.js or similar)
- [ ] Add quick stats: Total items listed, Items sold this quarter, Net payout

### Inventory Management UI
- [ ] Create inventory list view with DataTables or similar
- [ ] Implement inventory search and filtering (by status, location, SKU)
- [ ] Create add/edit inventory modal or page (with storage location dropdown)
- [ ] Add bulk import functionality (CSV upload)
- [ ] Implement inventory detail view
- [ ] Add photo upload for inventory items
- [ ] Create "Find Item" search by storage location
- [ ] Add visual storage location hierarchy tree
- [ ] Implement quick location change (drag-drop or dropdown)
- [ ] Show inventory status badges (Unlisted, Listed, Sold)

### Orders UI
- [ ] Create orders list view with filtering
- [ ] Implement order detail view with profit breakdown
- [ ] Add order status update functionality
- [ ] Create order search (by order ID, buyer, date)
- [ ] Display eBay fee breakdown per order

### Listings UI
- [ ] Create active listings view
- [ ] Implement listing creation form
- [ ] Add listing edit functionality
- [ ] Display listing performance metrics
- [ ] Implement end listing functionality

### Reports UI
- [ ] Create quarterly summary report page (matching spreadsheet tabs: Q3_2025, Q4_2025)
- [ ] Implement annual summary report for tax filing
- [ ] Create business expense tracking UI
  - [ ] Add expense entry form with category dropdown
  - [ ] Display expenses by category (Schedule C format)
  - [ ] Add expense category management
- [ ] Create mileage log UI
  - [ ] Mileage entry form (Date, Purpose, From, To, Miles)
  - [ ] Mileage log table with calculated deduction
  - [ ] Display total deduction by quarter/year
  - [ ] Show current IRS mileage rate
- [ ] Create profit margin analysis page
- [ ] Implement date range selectors for reports (Quarter, Year, Custom)
- [ ] Add export to CSV/Excel functionality (for accountant/tax prep)
- [ ] Create Schedule C preview report (categorized expenses)
- [ ] Add quarterly tax estimate calculator

### Forms & Validation
- [ ] Implement client-side validation with Bootstrap 5 validation styles
- [ ] Add server-side validation using FluentValidation
- [ ] Create reusable form components (date picker, currency input)
- [ ] Implement AJAX form submissions with error handling
- [ ] Add loading indicators for async operations

---

## CI/CD & DevOps

### GitHub Actions
- [ ] Create CI workflow for build and test on PR
- [ ] Set up automated testing in pipeline
- [ ] Configure code coverage reporting
- [ ] Implement CD workflow for Azure App Service deployment
- [ ] Add database migration step in deployment pipeline
- [ ] Configure environment-specific deployments (Staging, Production)
- [ ] Set up GitHub Secrets for Azure credentials and connection strings

### Azure Infrastructure
- [ ] Provision Azure App Service (Linux or Windows)
- [ ] Create Azure SQL Database
- [ ] Configure Application Insights for monitoring
- [ ] Set up Azure Key Vault for secrets management
- [ ] Configure connection strings in App Service settings
- [ ] Enable auto-scaling rules if needed
- [ ] Set up backup policies for Azure SQL

### Monitoring & Logging
- [ ] Configure Application Insights logging
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
- [ ] Configure TestContainers for SQL Server or use in-memory database
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

---

## Security & Performance

### Security
- [ ] Implement user authentication (ASP.NET Core Identity or Azure AD)
- [ ] Add authorization policies for sensitive operations
- [ ] Secure eBay API credentials (Key Vault, user secrets)
- [ ] Implement CSRF protection on forms
- [ ] Add input sanitization and XSS prevention
- [ ] Configure HTTPS redirection and HSTS

### Performance
- [ ] Implement caching strategy (memory cache, distributed cache)
- [ ] Optimize EF Core queries (avoid N+1, use projections)
- [ ] Add pagination to all list views
- [ ] Implement lazy loading or eager loading appropriately
- [ ] Profile and optimize database queries
- [ ] Add database indexes for common query patterns

---

## Documentation

- [ ] Update README.md with setup instructions
- [ ] Document eBay API setup and credentials
- [ ] Create architecture decision records (ADRs) for key decisions
- [ ] Document database schema
- [ ] Add XML documentation comments to public APIs
- [ ] Create user guide for application features

---

## Future Enhancements (Backlog)

- [ ] Multi-user support with role-based access (for business partners)
- [ ] Email notifications for sold items
- [ ] Automated pricing suggestions based on market data
- [ ] Integration with shipping carriers (USPS, UPS, FedEx) for real-time rates
- [ ] Mobile app or PWA version (for inventory lookup while sourcing)
- [ ] Barcode/QR code scanning for inventory management
- [ ] Advanced analytics and forecasting (predict quarterly income)
- [ ] Direct integration with TurboTax or H&R Block
- [ ] Integration with accounting software (QuickBooks, Xero)
- [ ] Automatic categorization of expenses using ML
- [ ] Receipt photo upload and OCR for expense tracking
- [ ] Inventory valuation methods (FIFO, LIFO, Average Cost)
- [ ] Sales tax tracking by state (for multi-state sellers)
- [ ] 1099-K form import and reconciliation
