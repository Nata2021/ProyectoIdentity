using ProyectoIdentity.Models.Domain.Enums; // Para los enums
using System.Collections.Generic; // Para colecciones de navegación
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;


namespace ProyectoIdentity.Models.Domain
{
    public class Caso
    {
        [Key]
        public int CasoId { get; set; }

        [Required]
        [StringLength(255)]
        public string Titulo { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Descripcion { get; set; }

        public DateTime FechaInicio { get; set; }

        public EstadoCaso Estado { get; set; } // Usando enum

        [Required]
        public int ClienteId { get; set; } // FK

        [Required]
        public int AbogadoId { get; set; } // FK

        // Propiedades de navegación
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        [ForeignKey("AbogadoId")]
        public Abogado Abogado { get; set; }

        // Nuevos campos sugeridos
        public TipoCaso TipoCaso { get; set; } // Usando enum

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaUltimaActualizacion { get; set; } // Nullable, se actualiza en el código

        public Prioridad Prioridad { get; set; } // Usando enum

        public bool IsDeleted { get; set; } = false; // Soft delete

        // Colecciones de navegación
        public ICollection<Documento> Documentos { get; set; }
        public ICollection<Tarea> Tareas { get; set; }
        public ICollection<Audiencia> Audiencias { get; set; }
        public ICollection<MovimientoFinanciero> MovimientosFinancieros { get; set; }
        public ICollection<RegistroTiempo> RegistrosDeTiempo { get; set; }
        public ICollection<Comunicacion> Comunicaciones { get; set; }
    }
}