using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class AddTestsSubmitDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActivationDate",
                table: "TestPackages",
                newName: "SubmitDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "NeoTests",
                newName: "SubmitDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "HollandsTests",
                newName: "SubmitDate");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CliftonTests",
                newName: "SubmitDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TestPackages",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitDate",
                table: "RavensTests",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TestPackages");

            migrationBuilder.DropColumn(
                name: "SubmitDate",
                table: "RavensTests");

            migrationBuilder.RenameColumn(
                name: "SubmitDate",
                table: "TestPackages",
                newName: "ActivationDate");

            migrationBuilder.RenameColumn(
                name: "SubmitDate",
                table: "NeoTests",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SubmitDate",
                table: "HollandsTests",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "SubmitDate",
                table: "CliftonTests",
                newName: "Name");
        }
    }
}
