using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class ChangesHealthFacilityId3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentHealthFacilityId",
                table: "Changes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationHealthFacilityId",
                table: "Changes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Changes_CurrentHealthFacilityId",
                table: "Changes",
                column: "CurrentHealthFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Changes_DestinationHealthFacilityId",
                table: "Changes",
                column: "DestinationHealthFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_HealthFacilities_CurrentHealthFacilityId",
                table: "Changes",
                column: "CurrentHealthFacilityId",
                principalTable: "HealthFacilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Changes_HealthFacilities_DestinationHealthFacilityId",
                table: "Changes",
                column: "DestinationHealthFacilityId",
                principalTable: "HealthFacilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Changes_HealthFacilities_CurrentHealthFacilityId",
                table: "Changes");

            migrationBuilder.DropForeignKey(
                name: "FK_Changes_HealthFacilities_DestinationHealthFacilityId",
                table: "Changes");

            migrationBuilder.DropIndex(
                name: "IX_Changes_CurrentHealthFacilityId",
                table: "Changes");

            migrationBuilder.DropIndex(
                name: "IX_Changes_DestinationHealthFacilityId",
                table: "Changes");

            migrationBuilder.DropColumn(
                name: "CurrentHealthFacilityId",
                table: "Changes");

            migrationBuilder.DropColumn(
                name: "DestinationHealthFacilityId",
                table: "Changes");
        }
    }
}
