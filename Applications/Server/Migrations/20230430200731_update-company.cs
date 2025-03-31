using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Application.Migrations
{
    /// <inheritdoc />
    public partial class updatecompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Companies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CompanyCode",
                table: "Companies",
                newName: "Kpp");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "Inn");

            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankBic",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BankBic",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Companies",
                newName: "CompanyName");

            migrationBuilder.RenameColumn(
                name: "Kpp",
                table: "Companies",
                newName: "CompanyCode");

            migrationBuilder.RenameColumn(
                name: "Inn",
                table: "Companies",
                newName: "Address");
        }
    }
}
