using Microsoft.EntityFrameworkCore.Migrations;

namespace Friends_Date_API.Migrations
{
    public partial class ChangeGenderColumnNameInUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "gender",
                table: "Users",
                newName: "Gender");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Users",
                newName: "gender");
        }
    }
}
