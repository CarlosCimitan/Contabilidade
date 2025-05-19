using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHistoricoId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "HistoricoId",
                table: "LancamentosContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_HistoricoId",
                table: "LancamentosContabeis",
                column: "HistoricoId");

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoId",
                table: "LancamentosContabeis",
                column: "HistoricoId",
                principalTable: "HistoricosContabeis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
