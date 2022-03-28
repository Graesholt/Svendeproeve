using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebAPI.Migrations
{
    public partial class MigrationV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    runId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false),
                    deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.runId);
                    table.ForeignKey(
                        name: "FK_Runs_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    pointId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    longitude = table.Column<double>(type: "float", nullable: false),
                    latitude = table.Column<double>(type: "float", nullable: false),
                    altitude = table.Column<double>(type: "float", nullable: true),
                    runId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.pointId);
                    table.ForeignKey(
                        name: "FK_Points_Runs_runId",
                        column: x => x.runId,
                        principalTable: "Runs",
                        principalColumn: "runId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Points_runId",
                table: "Points",
                column: "runId");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_userId",
                table: "Runs",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Runs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
