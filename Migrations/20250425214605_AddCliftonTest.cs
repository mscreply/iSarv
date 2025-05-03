using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class AddCliftonTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPsychologist",
                table: "AspNetUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TestPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FinalResult = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestPackages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CliftonTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    TestPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CliftonTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CliftonTests_TestPackages_TestPackageId",
                        column: x => x.TestPackageId,
                        principalTable: "TestPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HollandsTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    TestPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HollandsTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HollandsTests_TestPackages_TestPackageId",
                        column: x => x.TestPackageId,
                        principalTable: "TestPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NeoTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    TestPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NeoTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NeoTests_TestPackages_TestPackageId",
                        column: x => x.TestPackageId,
                        principalTable: "TestPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RavensTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Result = table.Column<string>(type: "TEXT", nullable: false),
                    TestPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RavensTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RavensTests_TestPackages_TestPackageId",
                        column: x => x.TestPackageId,
                        principalTable: "TestPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CliftonTestQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StatementA = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeA = table.Column<int>(type: "INTEGER", nullable: false),
                    StatementB = table.Column<string>(type: "TEXT", nullable: false),
                    ThemeB = table.Column<int>(type: "INTEGER", nullable: false),
                    Response = table.Column<int>(type: "INTEGER", nullable: false),
                    CliftonTestId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CliftonTestQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CliftonTestQuestions_CliftonTests_CliftonTestId",
                        column: x => x.CliftonTestId,
                        principalTable: "CliftonTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CliftonTestQuestions_CliftonTestId",
                table: "CliftonTestQuestions",
                column: "CliftonTestId");

            migrationBuilder.CreateIndex(
                name: "IX_CliftonTests_TestPackageId",
                table: "CliftonTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_HollandsTests_TestPackageId",
                table: "HollandsTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_NeoTests_TestPackageId",
                table: "NeoTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_RavensTests_TestPackageId",
                table: "RavensTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_TestPackages_UserId",
                table: "TestPackages",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CliftonTestQuestions");

            migrationBuilder.DropTable(
                name: "HollandsTests");

            migrationBuilder.DropTable(
                name: "NeoTests");

            migrationBuilder.DropTable(
                name: "RavensTests");

            migrationBuilder.DropTable(
                name: "CliftonTests");

            migrationBuilder.DropTable(
                name: "TestPackages");

            migrationBuilder.DropColumn(
                name: "IsPsychologist",
                table: "AspNetUsers");
        }
    }
}
