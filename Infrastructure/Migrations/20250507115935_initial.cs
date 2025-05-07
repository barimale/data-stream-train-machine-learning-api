using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.CreateTable(
                name: "cards",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegisteringTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModelAsBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Datas",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IngestionTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Xs = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ys = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PieceOfModel = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Datas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cards_Id",
                schema: "ordering",
                table: "cards",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "Datas",
                schema: "ordering");
        }
    }
}
