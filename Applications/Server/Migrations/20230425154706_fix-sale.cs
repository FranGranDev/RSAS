using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class fixsale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_StockId",
                table: "Sales",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Stocks_StockId",
                table: "Sales",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Stocks_StockId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_StockId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Sales");
        }
    }
}
