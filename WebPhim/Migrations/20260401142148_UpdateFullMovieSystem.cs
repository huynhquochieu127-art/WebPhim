using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebPhim.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFullMovieSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichChieus_Phongs_PhongId",
                table: "LichChieus");

            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_Raps_RapId",
                table: "Phongs");

            migrationBuilder.AddColumn<int>(
                name: "GheId1",
                table: "Ves",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "Raps",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DaoDien",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LoaiGhe",
                table: "Ghes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Ves_GheId1",
                table: "Ves",
                column: "GheId1");

            migrationBuilder.AddForeignKey(
                name: "FK_LichChieus_Phongs_PhongId",
                table: "LichChieus",
                column: "PhongId",
                principalTable: "Phongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_Raps_RapId",
                table: "Phongs",
                column: "RapId",
                principalTable: "Raps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ves_Ghes_GheId1",
                table: "Ves",
                column: "GheId1",
                principalTable: "Ghes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LichChieus_Phongs_PhongId",
                table: "LichChieus");

            migrationBuilder.DropForeignKey(
                name: "FK_Phongs_Raps_RapId",
                table: "Phongs");

            migrationBuilder.DropForeignKey(
                name: "FK_Ves_Ghes_GheId1",
                table: "Ves");

            migrationBuilder.DropIndex(
                name: "IX_Ves_GheId1",
                table: "Ves");

            migrationBuilder.DropColumn(
                name: "GheId1",
                table: "Ves");

            migrationBuilder.AlterColumn<string>(
                name: "DiaChi",
                table: "Raps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MoTa",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DaoDien",
                table: "Phims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiGhe",
                table: "Ghes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LichChieus_Phongs_PhongId",
                table: "LichChieus",
                column: "PhongId",
                principalTable: "Phongs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Phongs_Raps_RapId",
                table: "Phongs",
                column: "RapId",
                principalTable: "Raps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
