using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class fixorderstockid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
