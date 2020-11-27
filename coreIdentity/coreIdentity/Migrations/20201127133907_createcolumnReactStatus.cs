using Microsoft.EntityFrameworkCore.Migrations;

namespace coreIdentity.Migrations
{
    public partial class createcolumnReactStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReactStatus",
                table: "Statuss",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReactStatus",
                table: "Statuss");
        }
    }
}
