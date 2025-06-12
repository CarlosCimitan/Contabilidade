using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class addCodigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "LancamentosContabeis",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Codigo",
                table: "LancamentosContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Codigo",
                table: "HistoricosContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmpresaId",
                table: "HistoricosContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Codigo",
                table: "ContasContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosContabeis_EmpresaId",
                table: "HistoricosContabeis",
                column: "EmpresaId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosContabeis_Empresas_EmpresaId",
                table: "HistoricosContabeis",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosContabeis_Empresas_EmpresaId",
                table: "HistoricosContabeis");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosContabeis_EmpresaId",
                table: "HistoricosContabeis");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "LancamentosContabeis");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "LancamentosContabeis");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "HistoricosContabeis");

            migrationBuilder.DropColumn(
                name: "EmpresaId",
                table: "HistoricosContabeis");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "ContasContabeis");
        }
    }
}
