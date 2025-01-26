using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCore.Web.Persistence.Migrations
{
    public partial class AddClassroom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassroomId",
                table: "Course",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ClassroomId",
                table: "Course",
                column: "ClassroomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Classrooms_ClassroomId",
                table: "Course",
                column: "ClassroomId",
                principalTable: "Classrooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Classrooms_ClassroomId",
                table: "Course");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropIndex(
                name: "IX_Course_ClassroomId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ClassroomId",
                table: "Course");
        }
    }
}
