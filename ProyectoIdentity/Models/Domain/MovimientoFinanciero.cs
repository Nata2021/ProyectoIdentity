using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoMovimientoFinanciero

namespace ProyectoIdentity.Models.Domain
{
    public class MovimientoFinanciero
    {
        [Key]
        public int MovimientoId { get; set; }

        public int? CasoId { get; set; } // FK (nullable si un movimiento puede no estar ligado a un caso específico)
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        public TipoMovimientoFinanciero TipoMovimiento { get; set; } // 'Ingreso', 'Egreso', 'Honorarios', 'Gastos'

        [StringLength(255)]
        public string Concepto { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // Tipo de dato para moneda
        public decimal Monto { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.Now;

        public int? AbogadoId { get; set; } // Abogado que registró/aprobó el movimiento (nullable)
        [ForeignKey("AbogadoId")]
        public Abogado Abogado { get; set; }

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}