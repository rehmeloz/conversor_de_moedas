using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConversorMoedas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaTabUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Ativo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 23, 16, 31, 42, 241, DateTimeKind.Local).AddTicks(5562));

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 2,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 23, 16, 31, 42, 241, DateTimeKind.Local).AddTicks(5578));

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 3,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 23, 16, 31, 42, 241, DateTimeKind.Local).AddTicks(5579));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5625));

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 2,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5636));

            migrationBuilder.UpdateData(
                table: "Moedas",
                keyColumn: "Id",
                keyValue: 3,
                column: "UltimaAtualizacao",
                value: new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5638));
        }
    }
}
