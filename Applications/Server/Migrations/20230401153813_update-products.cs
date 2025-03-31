using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class updateproducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "WholesalePrice");

            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "StockProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RetailPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "StockProducts");

            migrationBuilder.DropColumn(
                name: "RetailPrice",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "WholesalePrice",
                table: "Products",
                newName: "Price");
        }
    }
}
