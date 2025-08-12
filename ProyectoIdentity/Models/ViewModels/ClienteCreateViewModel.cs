using System.ComponentModel.DataAnnotations;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoCliente

namespace ProyectoIdentity.Models.ViewModels
{
    public class ClienteCreateViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El DNI/CUIT/CUIL es obligatorio.")]
        [StringLength(20)]
        [Display(Name = "DNI / CUIT / CUIL")]
        public string DNI_CUIT_CUIL { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(255)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [StringLength(255)]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }

        [Display(Name = "Tipo de Cliente")]
        public TipoCliente TipoCliente { get; set; }

        [StringLength(255)]
        [Display(Name = "Razón Social (si es empresa)")]
        public string RazonSocial { get; set; }

        [Display(Name = "Notas Adicionales")]
        public string Notas { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}