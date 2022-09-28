using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class CVListDetailsAllowToEnroll : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AllowedReason",
                table: "CVListDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllowedToEnroll",
                table: "CVListDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowedReason",
                table: "CVListDetails");

            migrationBuilder.DropColumn(
                name: "AllowedToEnroll",
                table: "CVListDetails");
        }
    }
}
