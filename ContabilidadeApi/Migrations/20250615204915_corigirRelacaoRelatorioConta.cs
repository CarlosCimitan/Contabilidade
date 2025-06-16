using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class corigirRelacaoRelatorioConta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                table: "RelatoriosContas");

            migrationBuilder.AddForeignKey(
                name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                table: "RelatoriosContas",
                column: "ContaContabilId",
                principalTable: "ContasContabeis",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                table: "RelatoriosContas");

            migrationBuilder.AddForeignKey(
                name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                table: "RelatoriosContas",
                column: "ContaContabilId",
                principalTable: "ContasContabeis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
