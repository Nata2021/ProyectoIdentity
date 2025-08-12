using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable
using ProyectoIdentity.Models.Domain.Enums; // Para los enums de Tarea

namespace ProyectoIdentity.Models.ViewModels
{
    public class TareaCreateEditViewModel
    {
        // Para edición: el ID de la tarea
        public int TareaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un caso.")]
        [Display(Name = "Caso Asociado")]
        public int CasoId { get; set; }
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "La descripción de la tarea es obligatoria.")]
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha límite es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha Límite")]
        public DateTime FechaLimite { get; set; } = DateTime.Today;

        [Display(Name = "Estado")]
        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente; // Valor por defecto

        [Required(ErrorMessage = "Debe asignar la tarea a un abogado.")]
        [Display(Name = "Asignado A")]
        public int AsignadoA { get; set; } // AbogadoId
        public IEnumerable<SelectListItem> AbogadosList { get; set; } // Para el desplegable

        [Display(Name = "Tipo de Tarea")]
        public TipoTarea TipoTarea { get; set; } = TipoTarea.RedaccionEscrito; // Valor por defecto

        [Display(Name = "Prioridad")]
        public Prioridad Prioridad { get; set; } = Prioridad.Media; // Valor por defecto

        [Range(0, 100, ErrorMessage = "El progreso debe estar entre 0 y 100.")]
        [Display(Name = "Porcentaje de Progreso")]
        public int PorcentajeProgreso { get; set; } = 0;

        // FechaCompletado y CreadaPorUserId se manejan en el backend
        // IsDeleted se maneja en el backend (soft delete)
    }
}