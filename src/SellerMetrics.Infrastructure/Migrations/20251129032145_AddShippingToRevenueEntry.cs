using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddShippingToRevenueEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ShippingAmount",
                table: "RevenueEntries",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCurrency",
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
                name: "ShippingAmount",
                table: "RevenueEntries");

            migrationBuilder.DropColumn(
                name: "ShippingCurrency",
                table: "RevenueEntries");
        }
    }
}
