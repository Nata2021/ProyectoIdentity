using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoNotificacion
using ProyectoIdentity.Models.Domain; // Para AppUsuario

namespace ProyectoIdentity.Models.Domain // El namespace debe coincidir con la nueva ubicación (si moviste este archivo)
{
    public class Notificacion
    {
        [Key]
        public int NotificacionId { get; set; } // Convención: usar el nombre de la clase + Id

        [Required]
        [StringLength(450)] // Coincide con el tipo de Id de AspNetUsers/AppUsuario
        public string UserId { get; set; } // A quién va dirigida la notificación (renombrado de UsuarioId para claridad EF)

        // Propiedad de navegación al usuario de la aplicación
        [ForeignKey("UserId")]
        public AppUsuario User { get; set; } // Usar AppUsuario ya que es tu modelo extendido

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Mensaje { get; set; }

        // Campo para categorizar el tipo de notificación
        public TipoNotificacion TipoNotificacion { get; set; } // Ej. Nueva audiencia, Documento subido, Tarea asignada

        [StringLength(255)] // Puede ser el ID de un Caso, Tarea, Documento, etc. como string
        public string OrigenId { get; set; } // ID de la entidad relacionada (ej. "123", "CASO_ABC")

        public bool EsLeida { get; set; } = false; // Estado de lectura (renombrado de Leida para consistencia)

        public DateTime FechaNotificacion { get; set; } = DateTime.Now; // Fecha y hora de la notificación (renombrado de Fecha)

        public bool IsDeleted { get; set; } = false; // Soft delete, si decides implementarlo para notificaciones
    }
}