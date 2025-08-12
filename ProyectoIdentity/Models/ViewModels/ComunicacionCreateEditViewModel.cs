using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable

namespace ProyectoIdentity.Models.ViewModels
{
    public class ComunicacionCreateEditViewModel
    {
        // Para edición: el ID de la comunicación
        public int ComunicacionId { get; set; }

        [Display(Name = "Caso Asociado (Opcional)")]
        public int? CasoId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Display(Name = "Cliente Asociado (Opcional)")]
        public int? ClienteId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> ClientesList { get; set; } // Para el desplegable

        [Display(Name = "Abogado que Registra (Opcional)")]
        public int? AbogadoId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> AbogadosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "El tipo de comunicación es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Tipo de Comunicación")]
        public string TipoComunicacion { get; set; } // Ej. 'Llamada', 'Email', 'Reunion'

        [StringLength(255)]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; }

        [Display(Name = "Detalle")]
        [StringLength(1000)] // O el tamaño que consideres adecuado
        public string Detalle { get; set; }

        [Required(ErrorMessage = "La fecha y hora de la comunicación es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaComunicacion { get; set; } = DateTime.Now;

        // IsDeleted se maneja en el backend (soft delete)
    }
}