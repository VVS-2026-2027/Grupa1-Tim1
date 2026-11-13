using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Authorization;

namespace BentoLab.Controllers
{
    public class KorpasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KorpasController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");
            return View();
        }

        [Authorize]
        public IActionResult Create()
        {
            string[] cijene = TempData["CijenaKreiraneTorte"] as string[];
            double medjuzbir = 0;

            if (cijene != null)
            {
                foreach (var c in cijene)
                {
                    medjuzbir += double.Parse(c, CultureInfo.InvariantCulture);
                }
            }

            ViewBag.Medjuzbir = medjuzbir.ToString("F2", CultureInfo.InvariantCulture);

            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProcesirajPreuzimanje(string nacinPreuzimanja, string emailKupca, DateTime datumPreuzimanja)
        {
            TempData["DatumPreuzimanja"] = datumPreuzimanja.ToString("yyyy-MM-dd");
            TempData["EmailKupca"] = emailKupca;
            TempData["NacinPreuzimanja"] = nacinPreuzimanja;

            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");

            if (nacinPreuzimanja == "Dostava")
            {
                return RedirectToAction(nameof(Dostava));
            }

            KreirajNarudzbuIzKorpe(
            datumPreuzimanja,
            NacinPreuzimanja.LICNO_PREUZIMANJE,
            0
            );

            if (!string.IsNullOrEmpty(emailKupca))
            {
                string brojNarudzbe = "CK-" + new Random().Next(1000, 9999);
                EmailService.PosaljiObavjestenje(emailKupca, brojNarudzbe, "Zaprimljeno (Lično preuzimanje)");
            }

            return RedirectToAction(nameof(Uspjeh));
        }

        public IActionResult Dostava()
        {
            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");
            TempData.Keep("DatumPreuzimanja");
            TempData.Keep("EmailKupca");

            return View();
        }

        [HttpPost]
        public IActionResult PotvrdiDostavu(string ime, string adresa, string grad, string telefon, string napomena, string emailKupca)
        {
            DateTime datumPreuzimanja = DateTime.Now.AddDays(5);

            if (TempData["DatumPreuzimanja"] != null)
            {
                DateTime.TryParse(TempData["DatumPreuzimanja"].ToString(), out datumPreuzimanja);
            }

            KreirajNarudzbuIzKorpe(
            datumPreuzimanja,
            NacinPreuzimanja.DOSTAVA,
            5
            );

            if (!string.IsNullOrEmpty(emailKupca))
            {
                EmailService.PosaljiObavjestenje(emailKupca, "CK-0847", "Zaprimljeno");
            }

            return RedirectToAction(nameof(Uspjeh));
        }

        [HttpPost]
        public IActionResult Ukloni(int index)
        {
            string[] nazivi = TempData["NazivKreiraneTorte"] as string[];
            string[] cijene = TempData["CijenaKreiraneTorte"] as string[];

            if (nazivi != null && cijene != null && index >= 0 && index < nazivi.Length)
            {
                var novaListaNaziva = nazivi.ToList();
                var novaListaCijena = cijene.ToList();

                novaListaNaziva.RemoveAt(index);
                novaListaCijena.RemoveAt(index);

                TempData["NazivKreiraneTorte"] = novaListaNaziva.ToArray();
                TempData["CijenaKreiraneTorte"] = novaListaCijena.ToArray();
            }

            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Uspjeh()
        {
            return View();
        }

        private void KreirajNarudzbuIzKorpe(DateTime datumPreuzimanja, NacinPreuzimanja nacinPreuzimanja, double cijenaDostave)
        {
            string[] nazivi = TempData["NazivKreiraneTorte"] as string[];
            string[] cijene = TempData["CijenaKreiraneTorte"] as string[];

            if (nazivi == null || cijene == null || nazivi.Length == 0)
                return;

            int korisnikId = 0;
            string korisnikIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(korisnikIdString))
            {
                int.TryParse(korisnikIdString, out korisnikId);
            }

            if (korisnikId == 0)
            {
                var prviKorisnik = _context.Users.FirstOrDefault();
                if (prviKorisnik != null)
                    korisnikId = prviKorisnik.Id;
            }

            double ukupno = 0;

            foreach (var cijena in cijene)
            {
                ukupno += double.Parse(cijena, CultureInfo.InvariantCulture);
            }

            ukupno += cijenaDostave;

            var narudzba = new Narudzba
            {
                KorisnikID = korisnikId,
                UkupnaCijena = ukupno,
                KoeficijentSlozenosti = 1.25,
                Status = StatusNarudzbe.KREIRANA,
                DatumNarudzbe = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc),
                DatumPreuzimanja = DateTime.SpecifyKind(datumPreuzimanja, DateTimeKind.Utc),
                NacinPreuzimanja = nacinPreuzimanja
            };

            _context.Narudzbe.Add(narudzba);
            _context.SaveChanges();

            for (int i = 0; i < nazivi.Length; i++)
            {
                double cijenaStavke = double.Parse(cijene[i], CultureInfo.InvariantCulture);

                var torta = new Torta
                {
                    Naziv = nazivi[i],
                    Cijena = cijenaStavke,
                    KolicinaNaStanju = 1,
                    Dostupna = true
                };

                _context.Torta.Add(torta);
                _context.SaveChanges();

                var stavka = new StavkaNarudzbe
                {
                    Kolicina = 1,
                    CijenaStavke = cijenaStavke,
                    NarudzbaID = narudzba.NarudzbaID,
                    TortaID = torta.TortaID
                };

                _context.StavkeNarudzbe.Add(stavka);
            }

            _context.SaveChanges();

            TempData.Remove("NazivKreiraneTorte");
            TempData.Remove("CijenaKreiraneTorte");
            TempData.Remove("DatumPreuzimanja");
            TempData.Remove("EmailKupca");
            TempData.Remove("NacinPreuzimanja");
        }

        public static List<dynamic> PreuzmiIocistiKorpu(ITempDataDictionary tempData)
        {
            var listaStavki = new List<dynamic>();

            if (tempData != null)
            {
                string[] nazivi = tempData["NazivKreiraneTorte"] as string[];
                string[] cijene = tempData["CijenaKreiraneTorte"] as string[];

                if (nazivi != null && cijene != null)
                {
                    for (int i = 0; i < nazivi.Length; i++)
                    {
                        listaStavki.Add(new
                        {
                            NazivZaBazu = nazivi[i],
                            CijenaZaBazu = decimal.Parse(cijene[i], CultureInfo.InvariantCulture)
                        });
                    }
                }

                tempData.Remove("NazivKreiraneTorte");
                tempData.Remove("CijenaKreiraneTorte");
            }

            return listaStavki;
        }
    }
}
