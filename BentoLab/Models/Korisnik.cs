using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace BentoLab.Models
{
    public class Korisnik : IdentityUser<int>
    {
        [NotMapped]
        public int KorisnikID { get; set; }

        [NotMapped]
        public string Lozinka { get; set; }

        public string ImePrezime { get; set; }

        public Uloga Uloga { get; set; }

        public List<Narudzba> Narudzbe { get; set; }

        public Korpa Korpa { get; set; }

        public List<Obavjestenje> Obavjestenja { get; set; }

        public Korisnik()
        {
            ImePrezime = "Registrovani korisnik";
            Uloga = Uloga.KUPAC;
        }
    }
}