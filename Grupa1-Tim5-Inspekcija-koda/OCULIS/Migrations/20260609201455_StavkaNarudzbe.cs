using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCULIS.Migrations
{
    /// <inheritdoc />
    public partial class StavkaNarudzbe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StavkaNarudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNarudzba = table.Column<int>(type: "int", nullable: false),
                    IdProizvod = table.Column<int>(type: "int", nullable: false),
                    NazivProizvoda = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Kolicina = table.Column<int>(type: "int", nullable: false),
                    Cijena = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StavkaNarudzbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StavkaNarudzbe_Narudzba_IdNarudzba",
                        column: x => x.IdNarudzba,
                        principalTable: "Narudzba",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StavkaNarudzbe_Proizvod_IdProizvod",
                        column: x => x.IdProizvod,
                        principalTable: "Proizvod",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StavkaNarudzbe_IdNarudzba",
                table: "StavkaNarudzbe",
                column: "IdNarudzba");

            migrationBuilder.CreateIndex(
                name: "IX_StavkaNarudzbe_IdProizvod",
                table: "StavkaNarudzbe",
                column: "IdProizvod");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StavkaNarudzbe");
        }
    }
}
