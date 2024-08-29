using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddProgram : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BigGoals_Company_CompanyId",
                table: "BigGoals");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "BigGoals",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "ProgramBigGoal",
                columns: table => new
                {
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BigGoalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramBigGoal", x => new { x.BigGoalId, x.ProgramId });
                    table.ForeignKey(
                        name: "FK_ProgramBigGoal_BigGoals_BigGoalId",
                        column: x => x.BigGoalId,
                        principalTable: "BigGoals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramBigGoal_Program_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "Program",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramBigGoal_ProgramId",
                table: "ProgramBigGoal",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_BigGoals_Company_CompanyId",
                table: "BigGoals",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BigGoals_Company_CompanyId",
                table: "BigGoals");

            migrationBuilder.DropTable(
                name: "ProgramBigGoal");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "BigGoals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BigGoals_Company_CompanyId",
                table: "BigGoals",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
