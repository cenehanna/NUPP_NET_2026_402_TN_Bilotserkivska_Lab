using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Zoo.Infrastructure.Migrations
{
    [DbContext(typeof(ZooContext))]
    [Migration("20260910150000_AddKeepers")]
    public partial class AddKeepers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Keepers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keepers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnimalKeeper",
                columns: table => new
                {
                    AnimalsId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeepersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalKeeper", x => new { x.AnimalsId, x.KeepersId });
                    table.ForeignKey(
                        name: "FK_AnimalKeeper_Animals_AnimalsId",
                        column: x => x.AnimalsId,
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimalKeeper_Keepers_KeepersId",
                        column: x => x.KeepersId,
                        principalTable: "Keepers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalKeeper_KeepersId",
                table: "AnimalKeeper",
                column: "KeepersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimalKeeper");

            migrationBuilder.DropTable(
                name: "Keepers");
        }
    }
}
