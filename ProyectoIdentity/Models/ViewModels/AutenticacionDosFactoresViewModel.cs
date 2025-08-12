using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AutenticacionDosFactoresViewModel
    {
        [Required]
        [Display(Name = "Codigo del Autenticador")]
        public string Code { get; set; }
        public string Token { get; set; }

        // Codigo QR
        public string UrlCodigoQR { get; set; }

    }
}
