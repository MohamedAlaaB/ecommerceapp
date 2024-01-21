using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class fixbugcus01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orders_CustomerId",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_orders_CustomerId",
                table: "orders");

            migrationBuilder.CreateIndex(
                name: "IX_orders_CustomerId",
                table: "orders",
                column: "CustomerId",
                unique: true);
        }
    }
}
