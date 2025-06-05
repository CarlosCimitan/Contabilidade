using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMaskNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebitosCreditos_LancamentosContabeis_LancamentoContabilId",
                table: "DebitosCreditos");

            migrationBuilder.AddColumn<long>(
                name: "MascaraNumerica",
                table: "ContasContabeis",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_DebitosCreditos_LancamentosContabeis_LancamentoContabilId",
                table: "DebitosCreditos",
                column: "LancamentoContabilId",
                principalTable: "LancamentosContabeis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DebitosCreditos_LancamentosContabeis_LancamentoContabilId",
                table: "DebitosCreditos");

            migrationBuilder.DropColumn(
                name: "MascaraNumerica",
                table: "ContasContabeis");

            migrationBuilder.AddForeignKey(
                name: "FK_DebitosCreditos_LancamentosContabeis_LancamentoContabilId",
                table: "DebitosCreditos",
                column: "LancamentoContabilId",
                principalTable: "LancamentosContabeis",
                principalColumn: "Id");
        }
    }
}
