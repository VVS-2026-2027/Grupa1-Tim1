using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OCULIS.Data;

#nullable disable

namespace OCULIS.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260609120000_SpecifikacijaDopuna")]
    public partial class SpecifikacijaDopuna : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Proizvodjac",
                table: "Proizvod",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SlikaUrl",
                table: "Proizvod",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Poslovnica",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Poslovnica",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OsnovnaCijena",
                table: "Narudzba",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PopustPostotak",
                table: "Narudzba",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PopustIznos",
                table: "Narudzba",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "NacinPlacanja",
                table: "Placanje",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReferencaTransakcije",
                table: "Placanje",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PoslanoEmailom",
                table: "Obavijest",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BrojNarudzbi",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LojalnostBodovi",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Akcija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PopustPostotak = table.Column<double>(type: "float", nullable: false),
                    DatumPocetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktivna = table.Column<bool>(type: "bit", nullable: false),
                    IdProizvod = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Akcija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Akcija_Proizvod_IdProizvod",
                        column: x => x.IdProizvod,
                        principalTable: "Proizvod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Reklamacija",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Opis = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DatumPodnosenja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Odgovor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdKorisnik = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdNarudzba = table.Column<int>(type: "int", nullable: true),
                    IdElektronskiKarton = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reklamacija", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reklamacija_AspNetUsers_IdKorisnik",
                        column: x => x.IdKorisnik,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reklamacija_ElektronskiKarton_IdElektronskiKarton",
                        column: x => x.IdElektronskiKarton,
                        principalTable: "ElektronskiKarton",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reklamacija_Narudzba_IdNarudzba",
                        column: x => x.IdNarudzba,
                        principalTable: "Narudzba",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Akcija_IdProizvod",
                table: "Akcija",
                column: "IdProizvod");

            migrationBuilder.CreateIndex(
                name: "IX_Reklamacija_IdElektronskiKarton",
                table: "Reklamacija",
                column: "IdElektronskiKarton");

            migrationBuilder.CreateIndex(
                name: "IX_Reklamacija_IdKorisnik",
                table: "Reklamacija",
                column: "IdKorisnik");

            migrationBuilder.CreateIndex(
                name: "IX_Reklamacija_IdNarudzba",
                table: "Reklamacija",
                column: "IdNarudzba");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Akcija");
            migrationBuilder.DropTable(name: "Reklamacija");

            migrationBuilder.DropColumn(name: "Proizvodjac", table: "Proizvod");
            migrationBuilder.DropColumn(name: "SlikaUrl", table: "Proizvod");
            migrationBuilder.DropColumn(name: "Latitude", table: "Poslovnica");
            migrationBuilder.DropColumn(name: "Longitude", table: "Poslovnica");
            migrationBuilder.DropColumn(name: "OsnovnaCijena", table: "Narudzba");
            migrationBuilder.DropColumn(name: "PopustPostotak", table: "Narudzba");
            migrationBuilder.DropColumn(name: "PopustIznos", table: "Narudzba");
            migrationBuilder.DropColumn(name: "NacinPlacanja", table: "Placanje");
            migrationBuilder.DropColumn(name: "ReferencaTransakcije", table: "Placanje");
            migrationBuilder.DropColumn(name: "PoslanoEmailom", table: "Obavijest");
            migrationBuilder.DropColumn(name: "BrojNarudzbi", table: "AspNetUsers");
            migrationBuilder.DropColumn(name: "LojalnostBodovi", table: "AspNetUsers");
        }
    }
}
