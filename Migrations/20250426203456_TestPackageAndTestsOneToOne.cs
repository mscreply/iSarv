using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace iSarv.Migrations
{
    /// <inheritdoc />
    public partial class TestPackageAndTestsOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CliftonTests_TestPackages_TestPackageId",
                table: "CliftonTests");

            migrationBuilder.DropForeignKey(
                name: "FK_HollandsTests_TestPackages_TestPackageId",
                table: "HollandsTests");

            migrationBuilder.DropForeignKey(
                name: "FK_NeoTests_TestPackages_TestPackageId",
                table: "NeoTests");

            migrationBuilder.DropForeignKey(
                name: "FK_RavensTests_TestPackages_TestPackageId",
                table: "RavensTests");

            migrationBuilder.DropIndex(
                name: "IX_RavensTests_TestPackageId",
                table: "RavensTests");

            migrationBuilder.DropIndex(
                name: "IX_NeoTests_TestPackageId",
                table: "NeoTests");

            migrationBuilder.DropIndex(
                name: "IX_HollandsTests_TestPackageId",
                table: "HollandsTests");

            migrationBuilder.DropIndex(
                name: "IX_CliftonTests_TestPackageId",
                table: "CliftonTests");

            migrationBuilder.DropColumn(
                name: "TestPackageId",
                table: "RavensTests");

            migrationBuilder.DropColumn(
                name: "TestPackageId",
                table: "NeoTests");

            migrationBuilder.DropColumn(
                name: "TestPackageId",
                table: "HollandsTests");

            migrationBuilder.DropColumn(
                name: "TestPackageId",
                table: "CliftonTests");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RavensTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NeoTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HollandsTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CliftonTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddForeignKey(
                name: "FK_CliftonTests_TestPackages_Id",
                table: "CliftonTests",
                column: "Id",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HollandsTests_TestPackages_Id",
                table: "HollandsTests",
                column: "Id",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NeoTests_TestPackages_Id",
                table: "NeoTests",
                column: "Id",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RavensTests_TestPackages_Id",
                table: "RavensTests",
                column: "Id",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CliftonTests_TestPackages_Id",
                table: "CliftonTests");

            migrationBuilder.DropForeignKey(
                name: "FK_HollandsTests_TestPackages_Id",
                table: "HollandsTests");

            migrationBuilder.DropForeignKey(
                name: "FK_NeoTests_TestPackages_Id",
                table: "NeoTests");

            migrationBuilder.DropForeignKey(
                name: "FK_RavensTests_TestPackages_Id",
                table: "RavensTests");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "RavensTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TestPackageId",
                table: "RavensTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "NeoTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TestPackageId",
                table: "NeoTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HollandsTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TestPackageId",
                table: "HollandsTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CliftonTests",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "TestPackageId",
                table: "CliftonTests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RavensTests_TestPackageId",
                table: "RavensTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_NeoTests_TestPackageId",
                table: "NeoTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_HollandsTests_TestPackageId",
                table: "HollandsTests",
                column: "TestPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_CliftonTests_TestPackageId",
                table: "CliftonTests",
                column: "TestPackageId");

            migrationBuilder.AddForeignKey(
                name: "FK_CliftonTests_TestPackages_TestPackageId",
                table: "CliftonTests",
                column: "TestPackageId",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HollandsTests_TestPackages_TestPackageId",
                table: "HollandsTests",
                column: "TestPackageId",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NeoTests_TestPackages_TestPackageId",
                table: "NeoTests",
                column: "TestPackageId",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RavensTests_TestPackages_TestPackageId",
                table: "RavensTests",
                column: "TestPackageId",
                principalTable: "TestPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
