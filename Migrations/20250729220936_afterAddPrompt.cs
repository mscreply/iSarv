using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class afterAddPrompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PromptText = table.Column<string>(type: "TEXT", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    ReplyLength = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prompts");
        }
    }
}
