using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class AttBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaContabil_NaturezasConta_NaturezaContasId",
                table: "ContaContabil");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "NaturezasConta");

            migrationBuilder.DropIndex(
                name: "IX_RelatorioConta_ContaContabilId",
                table: "RelatorioConta");

            migrationBuilder.DropIndex(
                name: "IX_ContaContabil_NaturezaContasId",
                table: "ContaContabil");

            migrationBuilder.DropColumn(
                name: "Saldo",
                table: "ContaContabil");

            migrationBuilder.RenameColumn(
                name: "NaturezaContasId",
                table: "ContaContabil",
                newName: "NaturezaEnum");

            migrationBuilder.RenameColumn(
                name: "NaturezaContaId",
                table: "ContaContabil",
                newName: "LancamentoDebitoCreditoId");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "Usuarios",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LancamentoContabilId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "LancamentoDebitoCredito",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "LancamentoContabilId1",
                table: "LancamentoDebitoCredito",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HistoricoContabilId",
                table: "LancamentoContabil",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LancamentoDebitoCreditoID",
                table: "LancamentoContabil",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "LancamentoContabil",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LancamentoContabilId",
                table: "Empresas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Empresas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "HistoricoContabils",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<int>(type: "int", nullable: false),
                    Descricaio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LancamentoContabilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoContabils", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatorioConta_ContaContabilId",
                table: "RelatorioConta",
                column: "ContaContabilId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoDebitoCredito_ContaContabilId",
                table: "LancamentoDebitoCredito",
                column: "ContaContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoDebitoCredito_LancamentoContabilId1",
                table: "LancamentoDebitoCredito",
                column: "LancamentoContabilId1");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoContabil_HistoricoContabilId",
                table: "LancamentoContabil",
                column: "HistoricoContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoContabil_UsuarioId",
                table: "LancamentoContabil",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentoContabil_HistoricoContabils_HistoricoContabilId",
                table: "LancamentoContabil",
                column: "HistoricoContabilId",
                principalTable: "HistoricoContabils",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentoContabil_Usuarios_UsuarioId",
                table: "LancamentoContabil",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentoDebitoCredito_ContaContabil_ContaContabilId",
                table: "LancamentoDebitoCredito",
                column: "ContaContabilId",
                principalTable: "ContaContabil",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LancamentoDebitoCredito_LancamentoContabil_LancamentoContabilId1",
                table: "LancamentoDebitoCredito",
                column: "LancamentoContabilId1",
                principalTable: "LancamentoContabil",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LancamentoContabil_HistoricoContabils_HistoricoContabilId",
                table: "LancamentoContabil");

            migrationBuilder.DropForeignKey(
                name: "FK_LancamentoContabil_Usuarios_UsuarioId",
                table: "LancamentoContabil");

            migrationBuilder.DropForeignKey(
                name: "FK_LancamentoDebitoCredito_ContaContabil_ContaContabilId",
                table: "LancamentoDebitoCredito");

            migrationBuilder.DropForeignKey(
                name: "FK_LancamentoDebitoCredito_LancamentoContabil_LancamentoContabilId1",
                table: "LancamentoDebitoCredito");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "HistoricoContabils");

            migrationBuilder.DropIndex(
                name: "IX_RelatorioConta_ContaContabilId",
                table: "RelatorioConta");

            migrationBuilder.DropIndex(
                name: "IX_LancamentoDebitoCredito_ContaContabilId",
                table: "LancamentoDebitoCredito");

            migrationBuilder.DropIndex(
                name: "IX_LancamentoDebitoCredito_LancamentoContabilId1",
                table: "LancamentoDebitoCredito");

            migrationBuilder.DropIndex(
                name: "IX_LancamentoContabil_HistoricoContabilId",
                table: "LancamentoContabil");

            migrationBuilder.DropIndex(
                name: "IX_LancamentoContabil_UsuarioId",
                table: "LancamentoContabil");

            migrationBuilder.DropColumn(
                name: "LancamentoContabilId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "LancamentoContabilId1",
                table: "LancamentoDebitoCredito");

            migrationBuilder.DropColumn(
                name: "HistoricoContabilId",
                table: "LancamentoContabil");

            migrationBuilder.DropColumn(
                name: "LancamentoDebitoCreditoID",
                table: "LancamentoContabil");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "LancamentoContabil");

            migrationBuilder.DropColumn(
                name: "LancamentoContabilId",
                table: "Empresas");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Empresas");

            migrationBuilder.RenameColumn(
                name: "NaturezaEnum",
                table: "ContaContabil",
                newName: "NaturezaContasId");

            migrationBuilder.RenameColumn(
                name: "LancamentoDebitoCreditoId",
                table: "ContaContabil",
                newName: "NaturezaContaId");

            migrationBuilder.AlterColumn<int>(
                name: "EmpresaId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Valor",
                table: "LancamentoDebitoCredito",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "Saldo",
                table: "ContaContabil",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "NaturezasConta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Classificacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    grupo = table.Column<int>(type: "int", nullable: false),
                    natureza = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturezasConta", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RelatorioConta_ContaContabilId",
                table: "RelatorioConta",
                column: "ContaContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaContabil_NaturezaContasId",
                table: "ContaContabil",
                column: "NaturezaContasId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContaContabil_NaturezasConta_NaturezaContasId",
                table: "ContaContabil",
                column: "NaturezaContasId",
                principalTable: "NaturezasConta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Empresas_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId",
                principalTable: "Empresas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
