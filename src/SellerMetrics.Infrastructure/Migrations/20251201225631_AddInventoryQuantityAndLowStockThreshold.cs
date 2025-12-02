using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SellerMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryQuantityAndLowStockThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComponentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DefaultExpenseCategory = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsSystemType = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstimatedTaxPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaxYear = table.Column<int>(type: "integer", nullable: false),
                    Quarter = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EstimatedCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PaidCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsPaid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ConfirmationNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PaymentMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimatedTaxPayments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FiscalYearConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FiscalYearStartMonth = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYearConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IrsMileageRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    StandardRate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    MedicalRate = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    CharitableRate = table.Column<decimal>(type: "numeric(5,4)", nullable: true),
                    EffectiveDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IrsMileageRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceJobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CustomerContact = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    WaveInvoiceId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageLocations_StorageLocations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    BusinessLine = table.Column<int>(type: "integer", nullable: false),
                    Vendor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ReceiptPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ServiceJobId = table.Column<int>(type: "integer", nullable: true),
                    IsTaxDeductible = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ReferenceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessExpenses_ServiceJobs_ServiceJobId",
                        column: x => x.ServiceJobId,
                        principalTable: "ServiceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MileageEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TripDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Purpose = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StartLocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Miles = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IsRoundTrip = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BusinessLine = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ServiceJobId = table.Column<int>(type: "integer", nullable: true),
                    OdometerStart = table.Column<int>(type: "integer", nullable: true),
                    OdometerEnd = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MileageEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MileageEntries_ServiceJobs_ServiceJobId",
                        column: x => x.ServiceJobId,
                        principalTable: "ServiceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ComponentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentTypeId = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    UnitCostAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    UnitCostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    StorageLocationId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AcquiredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LowStockThreshold = table.Column<int>(type: "integer", nullable: true),
                    ServiceJobId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentItems_ComponentTypes_ComponentTypeId",
                        column: x => x.ComponentTypeId,
                        principalTable: "ComponentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComponentItems_ServiceJobs_ServiceJobId",
                        column: x => x.ServiceJobId,
                        principalTable: "ServiceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ComponentItems_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InternalSku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EbaySku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CogsAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CogsCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    PurchaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    StorageLocationId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Condition = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PhotoPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EbayListingId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ListedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    SoldDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_StorageLocations_StorageLocationId",
                        column: x => x.StorageLocationId,
                        principalTable: "StorageLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ComponentQuantityAdjustments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComponentItemId = table.Column<int>(type: "integer", nullable: false),
                    PreviousQuantity = table.Column<int>(type: "integer", nullable: false),
                    NewQuantity = table.Column<int>(type: "integer", nullable: false),
                    Adjustment = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    AdjustedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentQuantityAdjustments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentQuantityAdjustments_ComponentItems_ComponentItemId",
                        column: x => x.ComponentItemId,
                        principalTable: "ComponentItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevenueEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    EntryType = table.Column<int>(type: "integer", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    GrossAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    GrossCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    FeesAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FeesCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    EbayOrderId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    WaveInvoiceNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    InventoryItemId = table.Column<int>(type: "integer", nullable: true),
                    ServiceJobId = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenueEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevenueEntries_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RevenueEntries_ServiceJobs_ServiceJobId",
                        column: x => x.ServiceJobId,
                        principalTable: "ServiceJobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "ComponentTypes",
                columns: new[] { "Id", "CreatedAt", "DefaultExpenseCategory", "DeletedAt", "Description", "IsDeleted", "IsSystemType", "Name", "SortOrder", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 1, 22, 56, 31, 171, DateTimeKind.Utc).AddTicks(9697), "Parts & Materials", null, "Random Access Memory modules", false, true, "RAM", 1, null },
                    { 2, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1070), "Parts & Materials", null, "Solid State Drives", false, true, "SSD", 2, null },
                    { 3, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1073), "Parts & Materials", null, "Hard Disk Drives", false, true, "HDD", 3, null },
                    { 4, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1074), "Parts & Materials", null, "Power Supply Units (PSU)", false, true, "Power Supply", 4, null },
                    { 5, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1076), "Parts & Materials", null, "Central Processing Units", false, true, "CPU", 5, null },
                    { 6, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1077), "Parts & Materials", null, "System motherboards", false, true, "Motherboard", 6, null },
                    { 7, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1078), "Parts & Materials", null, "GPUs and video cards", false, true, "Graphics Card", 7, null },
                    { 8, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1080), "Parts & Materials", null, "Network interface cards and adapters", false, true, "Network Card", 8, null },
                    { 9, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1081), "Parts & Materials", null, "Fans, heatsinks, and cooling systems", false, true, "Cooling", 9, null },
                    { 10, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1082), "Parts & Materials", null, "Computer cases and enclosures", false, true, "Case", 10, null },
                    { 11, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1083), "Parts & Materials", null, "Cables and connectors", false, true, "Cables", 11, null },
                    { 12, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1085), "Parts & Materials", null, "Keyboards, mice, and other peripherals", false, true, "Peripherals", 12, null },
                    { 13, new DateTime(2025, 12, 1, 22, 56, 31, 172, DateTimeKind.Utc).AddTicks(1086), "Parts & Materials", null, "Other components", false, true, "Other", 99, null }
                });

            migrationBuilder.InsertData(
                table: "FiscalYearConfigurations",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "FiscalYearStartMonth", "IsActive", "IsDeleted", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, false, null });

            migrationBuilder.InsertData(
                table: "IrsMileageRates",
                columns: new[] { "Id", "CharitableRate", "CreatedAt", "DeletedAt", "EffectiveDate", "IsDeleted", "MedicalRate", "Notes", "StandardRate", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { 1, 0.14m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 0.21m, "IRS standard mileage rate for 2024", 0.67m, null, 2024 },
                    { 2, 0.14m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 0.21m, "IRS standard mileage rate for 2025", 0.70m, null, 2025 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_BusinessLine",
                table: "BusinessExpenses",
                column: "BusinessLine");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_Category",
                table: "BusinessExpenses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_ExpenseDate",
                table: "BusinessExpenses",
                column: "ExpenseDate");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_ExpenseDate_BusinessLine",
                table: "BusinessExpenses",
                columns: new[] { "ExpenseDate", "BusinessLine" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_ExpenseDate_Category",
                table: "BusinessExpenses",
                columns: new[] { "ExpenseDate", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_IsDeleted",
                table: "BusinessExpenses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessExpenses_ServiceJobId",
                table: "BusinessExpenses",
                column: "ServiceJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_ComponentTypeId",
                table: "ComponentItems",
                column: "ComponentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_IsDeleted",
                table: "ComponentItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_Quantity",
                table: "ComponentItems",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_ServiceJobId",
                table: "ComponentItems",
                column: "ServiceJobId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_Status",
                table: "ComponentItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentItems_StorageLocationId",
                table: "ComponentItems",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantityAdjustments_AdjustedAt",
                table: "ComponentQuantityAdjustments",
                column: "AdjustedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentQuantityAdjustments_ComponentItemId",
                table: "ComponentQuantityAdjustments",
                column: "ComponentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentTypes_IsDeleted",
                table: "ComponentTypes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentTypes_Name",
                table: "ComponentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComponentTypes_SortOrder",
                table: "ComponentTypes",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_EstimatedTaxPayments_IsPaid_DueDate",
                table: "EstimatedTaxPayments",
                columns: new[] { "IsPaid", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EstimatedTaxPayments_TaxYear_Quarter",
                table: "EstimatedTaxPayments",
                columns: new[] { "TaxYear", "Quarter" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYearConfigurations_IsActive",
                table: "FiscalYearConfigurations",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_EbayListingId",
                table: "InventoryItems",
                column: "EbayListingId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_EbaySku",
                table: "InventoryItems",
                column: "EbaySku");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InternalSku",
                table: "InventoryItems",
                column: "InternalSku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_IsDeleted",
                table: "InventoryItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_Status",
                table: "InventoryItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_StorageLocationId",
                table: "InventoryItems",
                column: "StorageLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_Title",
                table: "InventoryItems",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_IrsMileageRates_EffectiveDate",
                table: "IrsMileageRates",
                column: "EffectiveDate");

            migrationBuilder.CreateIndex(
                name: "IX_IrsMileageRates_Year",
                table: "IrsMileageRates",
                column: "Year");

            migrationBuilder.CreateIndex(
                name: "IX_IrsMileageRates_Year_EffectiveDate",
                table: "IrsMileageRates",
                columns: new[] { "Year", "EffectiveDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntries_BusinessLine",
                table: "MileageEntries",
                column: "BusinessLine");

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntries_IsDeleted",
                table: "MileageEntries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntries_ServiceJobId",
                table: "MileageEntries",
                column: "ServiceJobId");

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntries_TripDate",
                table: "MileageEntries",
                column: "TripDate");

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntries_TripDate_BusinessLine",
                table: "MileageEntries",
                columns: new[] { "TripDate", "BusinessLine" });

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_EbayOrderId",
                table: "RevenueEntries",
                column: "EbayOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_EntryType",
                table: "RevenueEntries",
                column: "EntryType");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_InventoryItemId",
                table: "RevenueEntries",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_IsDeleted",
                table: "RevenueEntries",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_ServiceJobId",
                table: "RevenueEntries",
                column: "ServiceJobId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_Source",
                table: "RevenueEntries",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_Source_TransactionDate",
                table: "RevenueEntries",
                columns: new[] { "Source", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_TransactionDate",
                table: "RevenueEntries",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_RevenueEntries_WaveInvoiceNumber",
                table: "RevenueEntries",
                column: "WaveInvoiceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_CustomerName",
                table: "ServiceJobs",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_IsDeleted",
                table: "ServiceJobs",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_JobNumber",
                table: "ServiceJobs",
                column: "JobNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_ReceivedDate",
                table: "ServiceJobs",
                column: "ReceivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_Status",
                table: "ServiceJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceJobs_WaveInvoiceId",
                table: "ServiceJobs",
                column: "WaveInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_IsDeleted",
                table: "StorageLocations",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_Name",
                table: "StorageLocations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_ParentId",
                table: "StorageLocations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageLocations_ParentId_SortOrder",
                table: "StorageLocations",
                columns: new[] { "ParentId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BusinessExpenses");

            migrationBuilder.DropTable(
                name: "ComponentQuantityAdjustments");

            migrationBuilder.DropTable(
                name: "EstimatedTaxPayments");

            migrationBuilder.DropTable(
                name: "FiscalYearConfigurations");

            migrationBuilder.DropTable(
                name: "IrsMileageRates");

            migrationBuilder.DropTable(
                name: "MileageEntries");

            migrationBuilder.DropTable(
                name: "RevenueEntries");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ComponentItems");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "ComponentTypes");

            migrationBuilder.DropTable(
                name: "ServiceJobs");

            migrationBuilder.DropTable(
                name: "StorageLocations");
        }
    }
}
