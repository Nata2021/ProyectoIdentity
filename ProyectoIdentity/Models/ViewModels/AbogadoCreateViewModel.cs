using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using ProyectoIdentity.Models.Enums;
using System.Collections.Generic; // Para IEnumerable
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AbogadoCreateViewModel
    {
        // Propiedades para los datos del Abogado
        // No incluyas AbogadoId aquí, ya que se genera automáticamente

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Nombre Completo del Abogado")]
        public string NombreCompleto { get; set; }

        [StringLength(50)]
        [Display(Name = "Número de Matrícula")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "El área de especialización es obligatoria.")]
        [Display(Name = "Área de Especialización")]
        public AreaEspecializacion AreaEspecializacion { get; set; }

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Formato de email profesional inválido.")]
        [Display(Name = "Email Profesional")]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Teléfono Profesional")]
        public string Telefono { get; set; }

        [Display(Name = "Activo en el Estudio")]
        public bool Activo { get; set; } = true;

        // Propiedades para la asociación con AppUsuario
        [Required(ErrorMessage = "Debe seleccionar un usuario para asociar al abogado.")]
        [Display(Name = "Usuario de la Aplicación")]
        public string UserId { get; set; } // Aquí almacenaremos el UserId seleccionado

        // Para poblar el dropdown de usuarios en la vista
        public IEnumerable<SelectListItem> UsuariosList { get; set; }
    }
}