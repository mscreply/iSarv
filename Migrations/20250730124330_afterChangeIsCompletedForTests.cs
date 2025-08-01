using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class afterChangeIsCompletedForTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "RavenTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "NeoTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "HollandTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "CliftonTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "RavenTests");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "NeoTests");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "HollandTests");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "CliftonTests");
        }
    }
}
