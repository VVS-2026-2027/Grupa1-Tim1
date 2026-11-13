using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace BentoLab.Controllers
{
    public class TortaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cjenovnik()
        {
            return View();
        }

        public IActionResult Galerija()
        {
            return View();
        }

        // Čim korisnik klikne da pravi NOVU tortu ispočetka,
        // čistimo zaostale torte od prošlog puta. Tako aplikacija kreće od nule
        public IActionResult Kreiraj()
        {
            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");

            return View();
        }

        [HttpPost]
        public IActionResult Kreiraj(string okus, string oblik, string boja)
        {
            TempData["IzabraniOkus"] = okus;
            TempData["IzabraniOblik"] = oblik;
            TempData["IzabranaBoja"] = boja;
            return RedirectToAction(nameof(Dekoracija));
        }

        public IActionResult Dekoracija()
        {
            TempData.Keep("IzabraniOkus");
            TempData.Keep("IzabraniOblik");
            TempData.Keep("IzabranaBoja");
            return View();
        }

        [HttpPost]
        public IActionResult Dekoracija(List<string> ukrasi, string slag, string dodaci, string napomena)
        {
            string okus = TempData["IzabraniOkus"]?.ToString() ?? "Bueno";
            string oblik = TempData["IzabraniOblik"]?.ToString() ?? "Krug";
            string boja = TempData["IzabranaBoja"]?.ToString() ?? "Preporuka slastičara";

            TempData["IzabraniUkrasi"] = ukrasi != null && ukrasi.Any() ? string.Join(", ", ukrasi) : "Nema ukrasa";
            TempData["IzabraniSlag"] = !string.IsNullOrEmpty(slag) ? slag : "Ništa";
            TempData["IzabraniDodaci"] = !string.IsNullOrEmpty(dodaci) ? dodaci : "Ništa od ponuđenog";
            TempData["Napomena"] = napomena;

            double osnovnaCijena = 20.0;
            if (okus == "Bueno" || okus == "Nutella") osnovnaCijena = 22.0;
            else if (okus == "Raffaelo") osnovnaCijena = 23.0;
            else if (okus == "Berry pistachio") osnovnaCijena = 25.0;

            double dodatnaCijena = 0;
            if (ukrasi != null)
            {
                foreach (var ukras in ukrasi)
                {
                    if (ukras == "Perlice" || ukras == "Šljokice") dodatnaCijena += 1.0;
                    else if (ukras == "Cvijeće") dodatnaCijena += 2.0;
                    else if (ukras == "Mašnice") dodatnaCijena += 3.0;
                    else if (ukras == "Figurica") dodatnaCijena += 5.0;
                }
            }
            if (slag != "Ništa" && !string.IsNullOrEmpty(slag)) dodatnaCijena += 1.0;
            if (dodaci != "Ništa od ponuđenog" && !string.IsNullOrEmpty(dodaci)) dodatnaCijena += 1.0;

            double koeficijent = 1.25;
            double konacnaCijena = (osnovnaCijena + dodatnaCijena) * koeficijent;

            TempData["OsnovnaCijena"] = osnovnaCijena.ToString("F2", CultureInfo.InvariantCulture);
            TempData["DodatnoCijena"] = dodatnaCijena.ToString("F2", CultureInfo.InvariantCulture);
            TempData["Koeficijent"] = koeficijent.ToString("F2", CultureInfo.InvariantCulture);
            TempData["UkupnaCijenaKonacna"] = konacnaCijena.ToString("F2", CultureInfo.InvariantCulture);

            TempData.Keep();
            return RedirectToAction(nameof(Procjena));
        }

        public IActionResult Procjena()
        {
            TempData.Keep();
            return View();
        }

        // Vraćeno na stabilni TempData sistem koji sigurno radi na klik
        [HttpPost]
        public IActionResult DodajUKorpu(string nazivTorte, string cijenaTorte)
        {
            List<string> naziviLista = TempData["NazivKreiraneTorte"] as string[] != null
            ? (TempData["NazivKreiraneTorte"] as string[]).ToList()
            : new List<string>();

            List<string> cijeneLista = TempData["CijenaKreiraneTorte"] as string[] != null
            ? (TempData["CijenaKreiraneTorte"] as string[]).ToList()
            : new List<string>();

            naziviLista.Add(nazivTorte);

            double parsiranaCijena = double.Parse(cijenaTorte, CultureInfo.InvariantCulture);
            cijeneLista.Add(parsiranaCijena.ToString("0.00", CultureInfo.InvariantCulture));

            TempData["NazivKreiraneTorte"] = naziviLista.ToArray();
            TempData["CijenaKreiraneTorte"] = cijeneLista.ToArray();

            TempData.Keep("NazivKreiraneTorte");
            TempData.Keep("CijenaKreiraneTorte");

            return RedirectToAction("Index", "Korpas");
        }
    }
}