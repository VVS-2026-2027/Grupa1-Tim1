using System;
using System.Net;
using System.Net.Mail;

namespace BentoLab.Controllers
{
    public class EmailService
    {
        // Dodali smo 'putanjaSlike' na kraj parametara
        public static void PosaljiObavjestenje(string primalacEmail, string brojNarudzbe, string noviStatus, string putanjaSlike = null)
        {
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("no-reply@bentolab.ba", "BentoLab Slastičarna");
                mailMessage.To.Add(primalacEmail);
                mailMessage.Subject = $"BentoLab - Status narudžbe #{brojNarudzbe}";
                mailMessage.IsBodyHtml = true;

                // Odabir ikone i poruke u zavisnosti od statusa
                string ikonica = "🎂";
                string glavnaPoruka = "Vaša narudžba je uspješno zaprimljena i poslana u kuhinju!";

                if (noviStatus.Contains("Spremno"))
                {
                    ikonica = "✨📸";
                    glavnaPoruka = "Slastičar je upravo završio vašu bento tortu i uslikao je za vas! Pogledajte kako izgleda:";
                }

                // DIO ZA SLIKU: Ako je slika poslana, pravimo HTML tag koji je prikazuje u mailu
                string slikaHtml = "";
                if (!string.IsNullOrEmpty(putanjaSlike))
                {
                    // Pošto Mailtrap radi lokalno, koristimo ugrađenu CID metodu za slanje slike kao priloga (Attachment)
                    // ili je povezujemo direktno ako imamo javni link. 

                    try
                    {
                        // Pravimo putanju do slike 
                        string stvarnaPutanjaNaDisku = AppDomain.CurrentDomain.BaseDirectory + "..\\..\\..\\wwwroot" + putanjaSlike;
                        Attachment inlineSlika = new Attachment(stvarnaPutanjaNaDisku);
                        inlineSlika.ContentId = "bento_slika";
                        inlineSlika.ContentDisposition.Inline = true;
                        inlineSlika.ContentDisposition.DispositionType = System.Net.Mime.DispositionTypeNames.Inline;
                        mailMessage.Attachments.Add(inlineSlika);

                        // HTML kod koji prikazuje sliku unutar maila
                        slikaHtml = $@"
                            <div style='margin: 20px 0; text-align: center;'>
                                <img src='cid:bento_slika' alt='Vaša Bento Torta' style='width: 300px; height: 300px; object-fit: cover; border-radius: 15px; border: 4px solid #b56c60;' />
                            </div>";
                    }
                    catch
                    {
                        // Ako se slika iz nekog razloga ne učita na disku, mail će se ipak poslati bez slike da aplikacija ne pukne
                        slikaHtml = "<p style='color: red;'>Slika torte se trenutno učitava...</p>";
                    }
                }

                // Kompletan HTML dizajn maila
                mailMessage.Body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 15px; background-color: #fcf8f7;'>
                    <div style='text-align: center;'>
                        <h1 style='color: #b56c60; margin-bottom: 5px;'>BentoLab 🎂</h1>
                        <p style='color: #a18885; text-uppercase; font-size: 12px; font-weight: bold; letter-spacing: 1px;'>Slastičarna iz snova</p>
                    </div>
                    <hr style='border: 0; height: 1px; background: #eee; margin: 20px 0;'>
                    <div style='text-align: center; padding: 10px 0;'>
                        <span style='font-size: 40px;'>{ikonica}</span>
                        <h3 style='color: #333; margin-top: 10px;'>Status: <span style='color: #b56c60;'>{noviStatus}</span></h3>
                        <p style='color: #666; font-size: 14px; line-height: 1.5; padding: 0 10px;'>{glavnaPoruka}</p>
                    </div>

                    {slikaHtml}

                    <div style='background-color: #fff; padding: 15px; border-radius: 10px; border: 1px solid #f0e6e4; margin-top: 20px;'>
                        <p style='margin: 0; color: #777; font-size: 12px;'>Broj narudžbe:</p>
                        <p style='margin: 5px 0 0 0; color: #b56c60; font-weight: bold; font-size: 16px;'>#{brojNarudzbe}</p>
                    </div>
                    <div style='text-align: center; margin-top: 25px; color: #a18885; font-size: 12px;'>
                        <p>Hvala Vam na povjerenju! <br> Vaš BentoLab tim Hrasno</p>
                    </div>
                </div>";

                // Mailtrap SMTP klijent
                var smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
                {
                    Port = 2525,
                    Credentials = new NetworkCredential("c96d47cfb6457d", "24ad7366fd2a4b"),
                    EnableSsl = true,
                };

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Zapisati grešku u konzolu ako slanje ne uspije
                Console.WriteLine("Greška pri slanju maila: " + ex.Message);
            }
        }
    }
}