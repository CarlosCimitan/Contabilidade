using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class VamosLa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Grau",
                table: "ContasContabeis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grau",
                table: "ContasContabeis");
        }
    }
}
