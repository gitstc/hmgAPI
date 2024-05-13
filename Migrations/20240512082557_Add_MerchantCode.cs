using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hmgAPI.Migrations
{
    /// <inheritdoc />
    public partial class Add_MerchantCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MerchantCode",
                table: "Merchants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MerchantCode",
                table: "Merchants");
        }
    }
}
