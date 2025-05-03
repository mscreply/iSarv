using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestsFieldForIsCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "RavensTests",
                newName: "Response");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "NeoTests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Response",
                table: "HollandsTests",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Response",
                table: "NeoTests");

            migrationBuilder.DropColumn(
                name: "Response",
                table: "HollandsTests");

            migrationBuilder.RenameColumn(
                name: "Response",
                table: "RavensTests",
                newName: "Name");
        }
    }
}
