using System.ComponentModel.DataAnnotations;

namespace OCULIS.Models.ViewModels
{
    public class TerminZakazivanjeViewModel
    {
        [Required(ErrorMessage = "Odaberite poslovnicu.")]
        [Display(Name = "Poslovnica")]
        public int IdPoslovnica { get; set; }

        [Required(ErrorMessage = "Odaberite datum.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum")]
        public DateTime Datum { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Odaberite vrijeme.")]
        [Display(Name = "Vrijeme")]
        public TimeSpan Vrijeme { get; set; }

        public List<TimeSpan> DostupniTermini { get; set; } = new();
    }
}
