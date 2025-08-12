using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoMovimientoFinanciero

namespace ProyectoIdentity.Models.ViewModels
{
    public class MovimientoFinancieroCreateEditViewModel
    {
        // Para edición: el ID del movimiento
        public int MovimientoId { get; set; }

        [Display(Name = "Caso Asociado (Opcional)")]
        public int? CasoId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> CasosList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "El tipo de movimiento es obligatorio.")]
        [Display(Name = "Tipo de Movimiento")]
        public TipoMovimientoFinanciero TipoMovimiento { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser un valor positivo.")]
        [Display(Name = "Monto")]
        [DataType(DataType.Currency)] // Para formato de moneda
        public decimal Monto { get; set; }

        [Display(Name = "Fecha del Movimiento")]
        [DataType(DataType.DateTime)]
        public DateTime FechaMovimiento { get; set; } = DateTime.Now;

        [Display(Name = "Abogado que Registra (Opcional)")]
        public int? AbogadoId { get; set; } // Puede ser nulo
        public IEnumerable<SelectListItem> AbogadosList { get; set; } // Para el desplegable

        // IsDeleted se maneja en el backend (soft delete)
    }
}
