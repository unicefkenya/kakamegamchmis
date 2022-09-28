using Microsoft.EntityFrameworkCore.Migrations;

namespace MCHMIS.Migrations
{
    public partial class ClinicVisitIdNotNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClinicVisitId",
                table: "MotherClinicVisits",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ClinicVisitId",
                table: "MotherClinicVisits",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
