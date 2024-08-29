using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class RemovePracticalActionFromDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PracticalActionIndicators");

            migrationBuilder.DropTable(
                name: "PracticalActions_Financials");

            migrationBuilder.DropTable(
                name: "Transitions_Financials");

            migrationBuilder.DropTable(
                name: "PracticalActions");

            migrationBuilder.CreateTable(
                name: "Financial",
                columns: table => new
                {
                    TransitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Financial", x => new { x.TransitionId, x.Id });
                    table.ForeignKey(
                        name: "FK_Financial_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Financial");

            migrationBuilder.CreateTable(
                name: "PracticalActions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LeaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperationalObjectiveId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Contractor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PracticalActions_OperationalObjectives_OperationalObjectiveId",
                        column: x => x.OperationalObjectiveId,
                        principalTable: "OperationalObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticalActions_People_LeaderId",
                        column: x => x.LeaderId,
                        principalTable: "People",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transitions_Financials",
                columns: table => new
                {
                    TransitionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transitions_Financials", x => new { x.TransitionId, x.Id });
                    table.ForeignKey(
                        name: "FK_Transitions_Financials_Transitions_TransitionId",
                        column: x => x.TransitionId,
                        principalTable: "Transitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalActionIndicators",
                columns: table => new
                {
                    IndicatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PracticalActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalActionIndicators", x => new { x.IndicatorId, x.PracticalActionId });
                    table.ForeignKey(
                        name: "FK_PracticalActionIndicators_Indicators_IndicatorId",
                        column: x => x.IndicatorId,
                        principalTable: "Indicators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PracticalActionIndicators_PracticalActions_PracticalActionId",
                        column: x => x.PracticalActionId,
                        principalTable: "PracticalActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PracticalActions_Financials",
                columns: table => new
                {
                    PracticalActionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PracticalActions_Financials", x => new { x.PracticalActionId, x.Id });
                    table.ForeignKey(
                        name: "FK_PracticalActions_Financials_PracticalActions_PracticalActionId",
                        column: x => x.PracticalActionId,
                        principalTable: "PracticalActions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PracticalActionIndicators_PracticalActionId",
                table: "PracticalActionIndicators",
                column: "PracticalActionId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalActions_LeaderId",
                table: "PracticalActions",
                column: "LeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PracticalActions_OperationalObjectiveId",
                table: "PracticalActions",
                column: "OperationalObjectiveId");
        }
    }
}
