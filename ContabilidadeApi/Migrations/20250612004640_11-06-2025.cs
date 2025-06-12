using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class _11062025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                table: "RelatoriosContas");

            migrationBuilder.DropIndex(
                name: "IX_RelatoriosContas_ContaContabilId",
                table: "RelatoriosContas");

            migrationBuilder.AddColumn<int>(
                name: "RelatorioContasId",
                table: "ContasContabeis",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelatorioId",
                table: "ContasContabeis",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContasContabeis_RelatorioContasId",
                table: "ContasContabeis",
                column: "RelatorioContasId");

            migrationBuilder.CreateIndex(
                name: "IX_ContasContabeis_RelatorioId",
                table: "ContasContabeis",
                column: "RelatorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContasContabeis_RelatoriosContas_RelatorioContasId",
                table: "ContasContabeis",
                column: "RelatorioContasId",
                principalTable: "RelatoriosContas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContasContabeis_RelatoriosContas_RelatorioId",
                table: "ContasContabeis",
                column: "RelatorioId",
                principalTable: "RelatoriosContas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContasContabeis_RelatoriosContas_RelatorioContasId",
                table: "ContasContabeis");

            migrationBuilder.DropForeignKey(
                name: "FK_ContasContabeis_RelatoriosContas_RelatorioId",
                table: "ContasContabeis");

            migrationBuilder.DropIndex(
                name: "IX_ContasContabeis_RelatorioContasId",
                table: "ContasContabeis");

            migrationBuilder.DropIndex(
                name: "IX_ContasContabeis_RelatorioId",
                table: "ContasContabeis");

            migrationBuilder.DropColumn(
                name: "RelatorioContasId",
                table: "ContasContabeis");

            migrationBuilder.DropColumn(
                name: "RelatorioId",
                table: "ContasContabeis");

            migrationBuilder.CreateIndex(
                name: "IX_RelatoriosContas_ContaContabilId",
                table: "RelatoriosContas",
                column: "ContaContabilId");

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
