using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoDocumento

namespace ProyectoIdentity.Models.Domain
{
    public class Documento
    {
        [Key]
        public int DocumentoId { get; set; }

        [Required]
        public int CasoId { get; set; } // FK

        // Propiedad de navegación
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        public TipoDocumento Tipo { get; set; } // Usando enum

        [Required]
        [StringLength(500)]
        public string RutaArchivo { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;

        // Nuevos campos sugeridos
        public int Version { get; set; } = 1; // Para control de versiones

        public int? DocumentoOriginalId { get; set; } // FK para enlazar versiones (nullable)
        [ForeignKey("DocumentoOriginalId")]
        public Documento DocumentoOriginal { get; set; } // Propiedad de navegación a la versión original

        [StringLength(450)] // Coincide con el tipo de Id de AspNetUsers
        public string SubidoPorUserId { get; set; } // Quién subió el documento
        [ForeignKey("SubidoPorUserId")]
        public AppUsuario SubidoPor { get; set; } // Propiedad de navegación al usuario que subió

        [Column(TypeName = "nvarchar(MAX)")]
        public string Tags { get; set; } // Palabras clave para búsqueda

        public bool IsPublic { get; set; } = false; // Si el documento es visible para clientes con acceso

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}