using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEbayIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EbayOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EbayOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LegacyOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BuyerUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ItemTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EbayItemId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    GrossSaleAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrossSaleCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ShippingPaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingPaidCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ShippingActualAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingActualCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    FinalValueFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinalValueFeeCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    PaymentProcessingFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentProcessingFeeCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    AdditionalFeesAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalFeesCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    InventoryItemId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    FulfillmentStatus = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EbayOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EbayOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EbayOrders_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EbayUserCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EbayUserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EbayUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EncryptedAccessToken = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EncryptedRefreshToken = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AccessTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Scopes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LastSyncedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSyncError = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EbayUserCredentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EbayUserCredentials_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_InventoryItemId",
                table: "EbayOrders",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_IsDeleted",
                table: "EbayOrders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_OrderDate",
                table: "EbayOrders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_Status",
                table: "EbayOrders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_UserId",
                table: "EbayOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EbayOrders_UserId_EbayOrderId",
                table: "EbayOrders",
                columns: new[] { "UserId", "EbayOrderId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EbayUserCredentials_IsConnected",
                table: "EbayUserCredentials",
                column: "IsConnected");

            migrationBuilder.CreateIndex(
                name: "IX_EbayUserCredentials_IsDeleted",
                table: "EbayUserCredentials",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_EbayUserCredentials_UserId",
                table: "EbayUserCredentials",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EbayOrders");

            migrationBuilder.DropTable(
                name: "EbayUserCredentials");
        }
    }
}
