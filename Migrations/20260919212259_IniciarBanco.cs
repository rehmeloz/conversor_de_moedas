using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConversorMoedas.Migrations
{
    /// <inheritdoc />
    public partial class IniciarBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MoedaOrigem = table.Column<string>(type: "TEXT", nullable: false),
                    MoedaDestino = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    Resultado = table.Column<decimal>(type: "TEXT", nullable: false),
                    TaxaUsada = table.Column<decimal>(type: "TEXT", nullable: false),
                    DataConversao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moedas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Taxa = table.Column<decimal>(type: "TEXT", nullable: false),
                    UltimaAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moedas", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Moedas",
                columns: new[] { "Id", "Codigo", "Nome", "Taxa", "UltimaAtualizacao" },
                values: new object[,]
                {
                    { 1, "USD", "Dólar Americano", 1.0m, new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5625) },
                    { 2, "BRL", "Real Brasileiro", 5.0m, new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5636) },
                    { 3, "EUR", "Euro", 0.92m, new DateTime(2026, 9, 19, 18, 22, 58, 817, DateTimeKind.Local).AddTicks(5638) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Conversoes");

            migrationBuilder.DropTable(
                name: "Moedas");
        }
    }
}
