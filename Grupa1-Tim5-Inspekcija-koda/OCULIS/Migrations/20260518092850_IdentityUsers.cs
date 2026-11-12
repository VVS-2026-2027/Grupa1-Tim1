using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OCULIS.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElektronskiKarton_Korisnik_IdKorisnik",
                table: "ElektronskiKarton");

            migrationBuilder.DropForeignKey(
                name: "FK_Korpa_Korisnik_IdKorisnik",
                table: "Korpa");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzba_Korisnik_IdKorisnik",
                table: "Narudzba");

            migrationBuilder.DropForeignKey(
                name: "FK_Obavijest_Korisnik_IdKorisnik",
                table: "Obavijest");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminPregleda_Korisnik_IdKorisnik",
                table: "TerminPregleda");

            migrationBuilder.DropTable(
                name: "Korisnik");

            migrationBuilder.AlterColumn<string>(
                name: "IdKorisnik",
                table: "TerminPregleda",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IdKorisnik",
                table: "Obavijest",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IdKorisnik",
                table: "Narudzba",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IdKorisnik",
                table: "Korpa",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "IdKorisnik",
                table: "ElektronskiKarton",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Ime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prezime",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telefon",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ElektronskiKarton_AspNetUsers_IdKorisnik",
                table: "ElektronskiKarton",
                column: "IdKorisnik",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Korpa_AspNetUsers_IdKorisnik",
                table: "Korpa",
                column: "IdKorisnik",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzba_AspNetUsers_IdKorisnik",
                table: "Narudzba",
                column: "IdKorisnik",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijest_AspNetUsers_IdKorisnik",
                table: "Obavijest",
                column: "IdKorisnik",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TerminPregleda_AspNetUsers_IdKorisnik",
                table: "TerminPregleda",
                column: "IdKorisnik",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElektronskiKarton_AspNetUsers_IdKorisnik",
                table: "ElektronskiKarton");

            migrationBuilder.DropForeignKey(
                name: "FK_Korpa_AspNetUsers_IdKorisnik",
                table: "Korpa");

            migrationBuilder.DropForeignKey(
                name: "FK_Narudzba_AspNetUsers_IdKorisnik",
                table: "Narudzba");

            migrationBuilder.DropForeignKey(
                name: "FK_Obavijest_AspNetUsers_IdKorisnik",
                table: "Obavijest");

            migrationBuilder.DropForeignKey(
                name: "FK_TerminPregleda_AspNetUsers_IdKorisnik",
                table: "TerminPregleda");

            migrationBuilder.DropColumn(
                name: "Ime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Prezime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Telefon",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "IdKorisnik",
                table: "TerminPregleda",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "IdKorisnik",
                table: "Obavijest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "IdKorisnik",
                table: "Narudzba",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "IdKorisnik",
                table: "Korpa",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "IdKorisnik",
                table: "ElektronskiKarton",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lozinka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ElektronskiKarton_Korisnik_IdKorisnik",
                table: "ElektronskiKarton",
                column: "IdKorisnik",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Korpa_Korisnik_IdKorisnik",
                table: "Korpa",
                column: "IdKorisnik",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Narudzba_Korisnik_IdKorisnik",
                table: "Narudzba",
                column: "IdKorisnik",
                principalTable: "Korisnik",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Obavijest_Korisnik_IdKorisnik",
                table: "Obavijest",
                column: "IdKorisnik",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TerminPregleda_Korisnik_IdKorisnik",
                table: "TerminPregleda",
                column: "IdKorisnik",
                principalTable: "Korisnik",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
