using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TipoPuja",
                table: "Puja",
                newName: "tipoPuja");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Puja",
                newName: "createdAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tipoPuja",
                table: "Puja",
                newName: "TipoPuja");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Puja",
                newName: "CreatedAt");
        }
    }
}
