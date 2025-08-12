using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para los enums

namespace ProyectoIdentity.Models.Domain
{
    public class Tarea
    {
        [Key]
        public int TareaId { get; set; }

        [Required]
        public int CasoId { get; set; } // FK

        // Propiedad de navegación
        [ForeignKey("CasoId")]
        public Caso Caso { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Descripcion { get; set; }

        public DateTime FechaLimite { get; set; }

        public EstadoTarea Estado { get; set; } // Usando enum

        [Required]
        public int AsignadoA { get; set; } // AbogadoId FK

        // Propiedad de navegación
        [ForeignKey("AsignadoA")]
        public Abogado AbogadoAsignado { get; set; }

        // Nuevos campos sugeridos
        public TipoTarea TipoTarea { get; set; } // Usando enum

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaCompletado { get; set; } // Nullable

        public Prioridad Prioridad { get; set; } // Usando enum

        [StringLength(450)] // Coincide con el tipo de Id de AspNetUsers
        public string CreadaPorUserId { get; set; } // Quién creó la tarea
        [ForeignKey("CreadaPorUserId")]
        public AppUsuario CreadaPor { get; set; }

        public int PorcentajeProgreso { get; set; } = 0; // Para tareas grandes

        public bool IsDeleted { get; set; } = false; // Soft delete
    }
}