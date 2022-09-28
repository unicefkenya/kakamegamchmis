using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class ChangesHealthFacilityId2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthFacilityId",
                table: "Changes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HealthFacilityId",
                table: "Changes",
                nullable: false,
                defaultValue: 0);
        }
    }
}
