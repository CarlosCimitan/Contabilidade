using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class CriandoBancoSqlServer : Migration
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
                name: "NaturezasConta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Classificacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    natureza = table.Column<int>(type: "int", nullable: false),
                    grupo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NaturezasConta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LancamentoContabil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Zeramento = table.Column<bool>(type: "bit", nullable: false),
                    DescComplementar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentoContabil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LancamentoContabil_Empresas_EmpresaId",
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
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    SenhaSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Cargo = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContaContabil",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mascara = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saldo = table.Column<double>(type: "float", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Situacao = table.Column<int>(type: "int", nullable: false),
                    TipoConta = table.Column<int>(type: "int", nullable: false),
                    NaturezaContaId = table.Column<int>(type: "int", nullable: false),
                    NaturezaContasId = table.Column<int>(type: "int", nullable: false),
                    EmpresaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContaContabil", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContaContabil_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContaContabil_NaturezasConta_NaturezaContasId",
                        column: x => x.NaturezaContasId,
                        principalTable: "NaturezasConta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LancamentoDebitoCredito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Valor = table.Column<double>(type: "float", nullable: false),
                    TipoAcao = table.Column<int>(type: "int", nullable: false),
                    LancamentoContabilId = table.Column<int>(type: "int", nullable: false),
                    ContaContabilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LancamentoDebitoCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LancamentoDebitoCredito_ContaContabil_LancamentoContabilId",
                        column: x => x.LancamentoContabilId,
                        principalTable: "ContaContabil",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LancamentoDebitoCredito_LancamentoContabil_LancamentoContabilId",
                        column: x => x.LancamentoContabilId,
                        principalTable: "LancamentoContabil",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RelatorioConta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Relatorio = table.Column<int>(type: "int", nullable: false),
                    ContaContabilId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelatorioConta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelatorioConta_ContaContabil_ContaContabilId",
                        column: x => x.ContaContabilId,
                        principalTable: "ContaContabil",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContaContabil_EmpresaId",
                table: "ContaContabil",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContaContabil_NaturezaContasId",
                table: "ContaContabil",
                column: "NaturezaContasId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoContabil_EmpresaId",
                table: "LancamentoContabil",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_LancamentoDebitoCredito_LancamentoContabilId",
                table: "LancamentoDebitoCredito",
                column: "LancamentoContabilId");

            migrationBuilder.CreateIndex(
                name: "IX_RelatorioConta_ContaContabilId",
                table: "RelatorioConta",
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
                name: "LancamentoDebitoCredito");

            migrationBuilder.DropTable(
                name: "RelatorioConta");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "LancamentoContabil");

            migrationBuilder.DropTable(
                name: "ContaContabil");

            migrationBuilder.DropTable(
                name: "Empresas");

            migrationBuilder.DropTable(
                name: "NaturezasConta");
        }
    }
}
