using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class CliftonTestQuestionDomainRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "CliftonTestQuestions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Domain",
                table: "CliftonTestQuestions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
