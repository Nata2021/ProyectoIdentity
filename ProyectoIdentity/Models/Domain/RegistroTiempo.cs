using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIdentity.Models.Domain
{
    public class RegistroTiempo
    {
        [Key]
        public int RegistroTiempoId { get; set; }

        [Required]
        public int AbogadoId { get; set; } // FK
        [ForeignKey("AbogadoId")]
        public Abogado Abogado { get; set; }

        public int? CasoId { get; set; } // FK (nullable si el tiempo no está ligado a un caso)
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now.Date; // Solo la fecha

        [Column(TypeName = "decimal(5, 2)")] // Formato para horas (ej. 8.50, 0.75)
        public decimal Horas { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string DescripcionActividad { get; set; }

        public bool Facturable { get; set; } = true; // Si estas horas pueden ser facturadas al cliente

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}