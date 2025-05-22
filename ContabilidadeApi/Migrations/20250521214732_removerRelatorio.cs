using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class removerRelatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Relatorios",
                table: "ContasContabeis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Relatorios",
                table: "ContasContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
