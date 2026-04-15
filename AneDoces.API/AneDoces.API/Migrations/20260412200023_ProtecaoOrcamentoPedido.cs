using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AneDoces.API.Migrations
{
    /// <inheritdoc />
    public partial class ProtecaoOrcamentoPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConvertidoEmPedido",
                table: "Orcamentos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "Orcamentos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertidoEmPedido",
                table: "Orcamentos");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "Orcamentos");
        }
    }
}
