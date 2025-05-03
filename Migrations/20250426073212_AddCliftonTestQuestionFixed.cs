using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class AddCliftonTestQuestionFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CliftonTestQuestions_CliftonTests_CliftonTestId",
                table: "CliftonTestQuestions");

            migrationBuilder.DropIndex(
                name: "IX_CliftonTestQuestions_CliftonTestId",
                table: "CliftonTestQuestions");

            migrationBuilder.DropColumn(
                name: "CliftonTestId",
                table: "CliftonTestQuestions");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "CliftonTestQuestions",
                newName: "Domain");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "CliftonTests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "CliftonTests");

            migrationBuilder.RenameColumn(
                name: "Domain",
                table: "CliftonTestQuestions",
                newName: "Response");

            migrationBuilder.AddColumn<int>(
                name: "CliftonTestId",
                table: "CliftonTestQuestions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CliftonTestQuestions_CliftonTestId",
                table: "CliftonTestQuestions",
                column: "CliftonTestId");

            migrationBuilder.AddForeignKey(
                name: "FK_CliftonTestQuestions_CliftonTests_CliftonTestId",
                table: "CliftonTestQuestions",
                column: "CliftonTestId",
                principalTable: "CliftonTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
