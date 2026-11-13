using System;
using System.IO;
using System.Threading.Tasks; // <- Potrebno za Task
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BentoLab.Controllers
{
    public class KameraController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        // Konstruktor za pristup wwwroot folderu
        public KameraController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // Početni ekran za slastičara
        public IActionResult Index()
        {
            return View();
        }

        // Ekran koji pali kameru za određenu narudžbu
        // Pristup u browseru: /Kamera/UslikajBento?narudzbaId=1
        public IActionResult UslikajBento(int narudzbaId)
        {
            ViewBag.NarudzbaId = narudzbaId;
            return View();
        }

        // Metoda koja prihvata uslikanu sliku i spašava je
        [HttpPost]
        public async Task<IActionResult> SpasiSliku(string base64Image, int narudzbaId)
        {
            if (string.IsNullOrEmpty(base64Image))
            {
                return BadRequest("Slikanje nije uspjelo.");
            }

            // 1. Dekodiranje i spašavanje slike na disk 
            var cistaSlika = base64Image.Replace("data:image/jpeg;base64,", "");
            byte[] bajtoviSlike = Convert.FromBase64String(cistaSlika);

            string folderPutanja = Path.Combine(_hostingEnvironment.WebRootPath, "slike_torta");
            if (!Directory.Exists(folderPutanja))
            {
                Directory.CreateDirectory(folderPutanja);
            }

            string imeFajla = $"bento_narudzba_{narudzbaId}.jpg";
            string kompletnaPutanja = Path.Combine(folderPutanja, imeFajla);
            await System.IO.File.WriteAllBytesAsync(kompletnaPutanja, bajtoviSlike); // Koristimo asinkrono pisanje

            // Relativna putanja koju spremamo u bazu i šaljemo servisu
            string putanjaZaMail = "/slike_torta/" + imeFajla;

            // Za potrebe testiranja, ovdje ćemo staviti testni mail:
            string emailKupca = "kupac.test@gmail.com";
            string testniBrojNarudzbe = "CK-0847";

            // 2. AUTOMATSKO OKIDANJE MAILA SA SLIKOM!
            EmailService.PosaljiObavjestenje(emailKupca, testniBrojNarudzbe, "Spremno za preuzimanje", putanjaZaMail);

            TempData["Poruka"] = "Uspješno uslikana bento torta i poslana obavijest sa slikom na e-mail!";

            // Vraća direktno na Admin Panel 
            return RedirectToAction("AdminPanel", "Home");
        }
    }
}