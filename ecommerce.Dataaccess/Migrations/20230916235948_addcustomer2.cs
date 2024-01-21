using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class addcustomer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer_Name",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Customer_PhoneNumber",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "creation_date",
                table: "orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer_Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                column: "CustomerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_CustomerId",
                table: "orders");

            migrationBuilder.DropTable(
                name: "customers");

            migrationBuilder.DropIndex(
                name: "IX_orders_CustomerId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "creation_date",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "Customer_Name",
                table: "orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Customer_PhoneNumber",
                table: "orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
