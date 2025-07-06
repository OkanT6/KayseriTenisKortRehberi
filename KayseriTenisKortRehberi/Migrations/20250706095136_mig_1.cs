using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KayseriTenisKortRehberi.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Facilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Facilities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Facilities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AddressModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TamAdres = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Il = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ilce = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mahalle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cadde = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Facilities_AddressId",
                table: "Facilities",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Facilities_AddressModel_AddressId",
                table: "Facilities",
                column: "AddressId",
                principalTable: "AddressModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Facilities_AddressModel_AddressId",
                table: "Facilities");

            migrationBuilder.DropTable(
                name: "AddressModel");

            migrationBuilder.DropIndex(
                name: "IX_Facilities_AddressId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Facilities");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Facilities");
        }
    }
}
