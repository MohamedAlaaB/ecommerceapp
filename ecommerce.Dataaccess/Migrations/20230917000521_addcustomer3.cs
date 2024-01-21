using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ecommerce.Dataaccess.Migrations
{
    /// <inheritdoc />
    public partial class addcustomer3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Customer_PhoneNumber",
                table: "customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Customer_PhoneNumber",
                table: "customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
