using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Narudzba
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Datum narudžbe")]
        public DateTime DatumNarudzbe { get; set; }

        [Display(Name = "Status")]
        public StatusNarudzbe Status { get; set; }

        [Display(Name = "Osnovna cijena")]
        public double OsnovnaCijena { get; set; }

        [Display(Name = "Popust (%)")]
        public double PopustPostotak { get; set; }

        [Display(Name = "Iznos popusta")]
        public double PopustIznos { get; set; }

        [Display(Name = "Ukupna cijena")]
        public double UkupnaCijena { get; set; }

        [Required(ErrorMessage = "Adresa isporuke je obavezna.")]
        [StringLength(250, MinimumLength = 5, ErrorMessage = "Adresa mora imati između 5 i 250 karaktera.")]
        [RegularExpression(@"^(?=.*[A-Za-zČĆŽŠĐčćžšđ])(?=.*\d)[A-Za-zČĆŽŠĐčćžšđ0-9\s\.\-\/,]+$",
            ErrorMessage = "Adresa mora sadržavati naziv ulice i broj, bez specijalnih znakova.")]
        [Display(Name = "Adresa isporuke")]
        public string AdresaIsporuke { get; set; } = string.Empty;

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;

        public Korisnik Korisnik { get; set; } = null!;

        [ForeignKey(nameof(Korpa))]
        public int IdKorpa { get; set; }

        public Korpa Korpa { get; set; } = null!;

        public ICollection<StavkaNarudzbe> Stavke { get; set; } = new List<StavkaNarudzbe>();

        public ICollection<Placanje> Placanja { get; set; } = new List<Placanje>();

        public ICollection<Reklamacija> Reklamacije { get; set; } = new List<Reklamacija>();
    }
}