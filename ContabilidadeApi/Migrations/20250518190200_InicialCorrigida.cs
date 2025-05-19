using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class InicialCorrigida : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CNPJ = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosContabeis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosContabeis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContasContabeis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mascara = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Situacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoConta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Natureza = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContasContabeis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContasContabeis_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SenhaSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    TokenDataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelatoriosContas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Relatorio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContaContabilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatoriosContas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatoriosContas_ContasContabeis_ContaContabilId",
                        column: x => x.ContaContabilId,
                        principalTable: "ContasContabeis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LancamentosContabeis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Zeramento = table.Column<bool>(type: "bit", nullable: false),
                    DescComplementar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    HistoricoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentosContabeis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LancamentosContabeis_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LancamentosContabeis_HistoricosContabeis_HistoricoId",
                        column: x => x.HistoricoId,
                        principalTable: "HistoricosContabeis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LancamentosContabeis_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DebitosCreditos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    TipoAcao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescComplementar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContaContabilId = table.Column<int>(type: "int", nullable: false),
                    LancamentoContabilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitosCreditos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebitosCreditos_ContasContabeis_ContaContabilId",
                        column: x => x.ContaContabilId,
                        principalTable: "ContasContabeis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DebitosCreditos_LancamentosContabeis_LancamentoContabilId",
                        column: x => x.LancamentoContabilId,
                        principalTable: "LancamentosContabeis",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContasContabeis_EmpresaId",
                table: "ContasContabeis",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitosCreditos_ContaContabilId",
                table: "DebitosCreditos",
                column: "ContaContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitosCreditos_LancamentoContabilId",
                table: "DebitosCreditos",
                column: "LancamentoContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_EmpresaId",
                table: "LancamentosContabeis",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_HistoricoId",
                table: "LancamentosContabeis",
                column: "HistoricoId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentosContabeis_UsuarioId",
                table: "LancamentosContabeis",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatoriosContas_ContaContabilId",
                table: "RelatoriosContas",
                column: "ContaContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmpresaId",
                table: "Usuarios",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebitosCreditos");

            migrationBuilder.DropTable(
                name: "RelatoriosContas");

            migrationBuilder.DropTable(
                name: "LancamentosContabeis");

            migrationBuilder.DropTable(
                name: "ContasContabeis");

            migrationBuilder.DropTable(
                name: "HistoricosContabeis");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
