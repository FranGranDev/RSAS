using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class fixdelivery : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDelivery_Orders_Id",
                table: "OrdersDelivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdersDelivery",
                table: "OrdersDelivery");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OrdersDelivery");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdersDelivery",
                table: "OrdersDelivery",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDelivery_Orders_OrderId",
                table: "OrdersDelivery",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrdersDelivery_Orders_OrderId",
                table: "OrdersDelivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdersDelivery",
                table: "OrdersDelivery");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OrdersDelivery",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdersDelivery",
                table: "OrdersDelivery",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrdersDelivery_Orders_Id",
                table: "OrdersDelivery",
                column: "Id",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
