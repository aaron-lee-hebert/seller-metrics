using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaxesToRevenueEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TaxesCollectedAmount",
                table: "RevenueEntries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TaxesCollectedCurrency",
                table: "RevenueEntries",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxesCollectedAmount",
                table: "RevenueEntries");

            migrationBuilder.DropColumn(
                name: "TaxesCollectedCurrency",
                table: "RevenueEntries");
        }
    }
}
