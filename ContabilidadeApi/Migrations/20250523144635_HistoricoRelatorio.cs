using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class HistoricoRelatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HistoricoId",
                table: "LancamentosContabeis",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_HistoricoId",
                table: "LancamentosContabeis",
                column: "HistoricoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoId",
                table: "LancamentosContabeis",
                column: "HistoricoId",
                principalTable: "HistoricosContabeis",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoId",
                table: "LancamentosContabeis");

            migrationBuilder.DropIndex(
                name: "IX_LancamentosContabeis_HistoricoId",
                table: "LancamentosContabeis");

            migrationBuilder.DropColumn(
                name: "HistoricoId",
                table: "LancamentosContabeis");
        }
    }
}
