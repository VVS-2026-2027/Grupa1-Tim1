using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BentoLab.Migrations
{
    /// <inheritdoc />
    public partial class DodanaCijenaTorte : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Cijena",
                table: "Torta",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cijena",
                table: "Torta");
        }
    }
}
