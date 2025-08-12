using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoAudiencia

namespace ProyectoIdentity.Models.ViewModels
{
    public class AudienciaCreateEditViewModel
    {
        // Para edición: el ID de la audiencia
        public int AudienciaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un caso.")]
        [Display(Name = "Caso Asociado")]
        public int CasoId { get; set; }
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "La fecha y hora de la audiencia es obligatoria.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora")]
        public DateTime Fecha { get; set; } = DateTime.Now; // Valor por defecto: fecha y hora actual

        [StringLength(255)]
        [Display(Name = "Lugar")]
        public string Lugar { get; set; }

        [Display(Name = "Notas")]
        public string Notas { get; set; }

        [Display(Name = "Tipo de Audiencia")]
        public TipoAudiencia TipoAudiencia { get; set; } = TipoAudiencia.Preliminar; // Valor por defecto

        [StringLength(255)]
        [Display(Name = "Juez")]
        public string Juez { get; set; }

        [StringLength(255)]
        [Display(Name = "Tribunal")]
        public string Tribunal { get; set; }

        [Display(Name = "¿Recordatorio Enviado?")]
        public bool EsRecordatorioEnviado { get; set; } = false;

        // IsDeleted se maneja en el backend (soft delete)
    }
}