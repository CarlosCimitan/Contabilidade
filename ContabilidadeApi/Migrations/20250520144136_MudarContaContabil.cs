using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class MudarContaContabil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoContabilId",
                table: "LancamentosContabeis");

            migrationBuilder.DropIndex(
                name: "IX_LancamentosContabeis_HistoricoContabilId",
                table: "LancamentosContabeis");

            migrationBuilder.DropColumn(
                name: "HistoricoContabilId",
                table: "LancamentosContabeis");

            migrationBuilder.AddColumn<int>(
                name: "Relatorios",
                table: "ContasContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Relatorios",
                table: "ContasContabeis");

            migrationBuilder.AddColumn<int>(
                name: "HistoricoContabilId",
                table: "LancamentosContabeis",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_HistoricoContabilId",
                table: "LancamentosContabeis",
                column: "HistoricoContabilId");

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoContabilId",
                table: "LancamentosContabeis",
                column: "HistoricoContabilId",
                principalTable: "HistoricosContabeis",
                principalColumn: "Id");
        }
    }
}
