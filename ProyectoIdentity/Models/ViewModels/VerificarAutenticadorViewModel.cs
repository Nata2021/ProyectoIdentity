using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class VerificarAutenticadorViewModel
    {

        [Required]
        [Display(Name = "Codigo del Autenticador")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Recordar datos?")]

        public bool RecordarDatos { get; set; }
    }
}
