using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SellerMetrics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryQuantityAndLowStockThreshold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InventoryItems",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "LowStockThreshold",
                table: "ComponentItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InventoryItems");

            migrationBuilder.AddColumn<int>(
                name: "LowStockThreshold",
                table: "ComponentItems",
                type: "integer",
                nullable: true);
        }
    }
}
