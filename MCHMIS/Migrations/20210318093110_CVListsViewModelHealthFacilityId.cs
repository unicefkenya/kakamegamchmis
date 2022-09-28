using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class CVListsViewModelHealthFacilityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CVLists_HealthFacilityId",
                table: "CVLists",
                column: "HealthFacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CVLists_HealthFacilities_HealthFacilityId",
                table: "CVLists",
                column: "HealthFacilityId",
                principalTable: "HealthFacilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CVLists_HealthFacilities_HealthFacilityId",
                table: "CVLists");

            migrationBuilder.DropIndex(
                name: "IX_CVLists_HealthFacilityId",
                table: "CVLists");
        }
    }
}
