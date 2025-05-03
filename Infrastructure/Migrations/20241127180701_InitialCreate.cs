using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Card.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    AccountNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PIN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SerialNumber = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cards_AccountNumber",
                schema: "ordering",
                table: "cards",
                column: "AccountNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cards_Id",
                schema: "ordering",
                table: "cards",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cards_SerialNumber",
                schema: "ordering",
                table: "cards",
                column: "SerialNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cards",
                schema: "ordering");
        }
    }
}
