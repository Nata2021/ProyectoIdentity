using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProyectoIdentity.Models.Domain.Enums; // Para el enum TipoCliente
using System.Collections.Generic; // Para colecciones de navegación

namespace ProyectoIdentity.Models.Domain
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [StringLength(255)]
        public string NombreCompleto { get; set; }

        [StringLength(20)]
        [Required]
        public string DNI_CUIT_CUIL { get; set; } // Mejor nombre para incluir identificación fiscal

        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Direccion { get; set; }

        // Nuevos campos sugeridos
        public TipoCliente TipoCliente { get; set; } // Persona Física o Empresa (usando enum)

        [StringLength(255)]
        public string RazonSocial { get; set; } // Si TipoCliente es Empresa

        [Column(TypeName = "nvarchar(MAX)")] // Para permitir texto largo
        public string Notas { get; set; }

        public DateTime FechaAlta { get; set; } = DateTime.Now; // Fecha de creación del cliente

        public bool Activo { get; set; } = true; // Para soft delete

        // Colecciones de navegación
        public ICollection<Caso> Casos { get; set; }
        public ICollection<Comunicacion> Comunicaciones { get; set; }
    }
}