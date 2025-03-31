using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class addstockidorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StockId",
                table: "Orders",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetDefault);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Stocks_StockId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StockId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "Orders");
        }
    }
}
