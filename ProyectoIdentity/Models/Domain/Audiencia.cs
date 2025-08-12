using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoAudiencia

namespace ProyectoIdentity.Models.Domain
{
    public class Audiencia
    {
        [Key]
        public int AudienciaId { get; set; }

        [Required]
        public int CasoId { get; set; } // FK

        // Propiedad de navegación
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        public DateTime Fecha { get; set; } // DATETIME ya incluye fecha y hora

        [StringLength(255)]
        public string Lugar { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Notas { get; set; }

        // Nuevos campos sugeridos
        public TipoAudiencia TipoAudiencia { get; set; } // Usando enum

        [StringLength(255)]
        public string Juez { get; set; }

        [StringLength(255)]
        public string Tribunal { get; set; }

        public bool EsRecordatorioEnviado { get; set; } = false;

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}