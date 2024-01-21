using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class orderupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "employeename",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "employeename",
                table: "orders");
        }
    }
}
