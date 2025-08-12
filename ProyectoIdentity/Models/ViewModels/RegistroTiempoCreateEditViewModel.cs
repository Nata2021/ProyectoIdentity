using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable

namespace ProyectoIdentity.Models.ViewModels
{
    public class RegistroTiempoCreateEditViewModel
    {
        // Para edición: el ID del registro de tiempo
        public int RegistroTiempoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un abogado.")]
        [Display(Name = "Abogado")]
        public int AbogadoId { get; set; }
        public IEnumerable<SelectListItem> AbogadosList { get; set; } // Para el desplegable

        [Display(Name = "Caso Asociado (Opcional)")]
        public int? CasoId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Las horas son obligatorias.")]
        [Range(0.01, 24.00, ErrorMessage = "Las horas deben ser un valor entre 0.01 y 24.00.")]
        [Display(Name = "Horas Dedicadas")]
        public decimal Horas { get; set; }

        [Display(Name = "Descripción de la Actividad")]
        [StringLength(500)]
        public string DescripcionActividad { get; set; }

        [Display(Name = "Facturable")]
        public bool Facturable { get; set; } = true;

        // IsDeleted se maneja en el backend (soft delete)
    }
}