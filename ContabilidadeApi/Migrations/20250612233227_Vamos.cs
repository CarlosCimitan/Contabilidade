using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class Vamos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "saldo",
                table: "ContasContabeis",
                newName: "Saldo");

            migrationBuilder.AlterColumn<double>(
                name: "Saldo",
                table: "ContasContabeis",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Saldo",
                table: "ContasContabeis",
                newName: "saldo");

            migrationBuilder.AlterColumn<float>(
                name: "saldo",
                table: "ContasContabeis",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
