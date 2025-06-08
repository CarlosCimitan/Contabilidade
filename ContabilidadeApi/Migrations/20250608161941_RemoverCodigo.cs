using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContabilidadeApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoverCodigo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "ContasContabeis");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "ContasContabeis",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
