using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIdentity.Models.Domain
{
    public class Comunicacion
    {
        [Key]
        public int ComunicacionId { get; set; }

        public int? CasoId { get; set; } // FK (nullable)
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        public int? ClienteId { get; set; } // FK (nullable)
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        public int? AbogadoId { get; set; } // Abogado que registró la comunicación (nullable)
        [ForeignKey("AbogadoId")]
        public Abogado Abogado { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoComunicacion { get; set; } // Ej. 'Llamada', 'Email', 'Reunion', 'Mensaje'

        [StringLength(255)]
        public string Asunto { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Detalle { get; set; }

        public DateTime FechaComunicacion { get; set; } = DateTime.Now;

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}