using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models
{
    public class ConfirmarCuentaExternaViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
