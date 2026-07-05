using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJoinProject_Employees_EmployeeId",
                table: "EmployeeJoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJoinProject_Projects_ProjectId",
                table: "EmployeeJoinProject");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJoinProject_Employees_EmployeeId",
                table: "EmployeeJoinProject",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJoinProject_Projects_ProjectId",
                table: "EmployeeJoinProject",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJoinProject_Employees_EmployeeId",
                table: "EmployeeJoinProject");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeJoinProject_Projects_ProjectId",
                table: "EmployeeJoinProject");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJoinProject_Employees_EmployeeId",
                table: "EmployeeJoinProject",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeJoinProject_Projects_ProjectId",
                table: "EmployeeJoinProject",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
