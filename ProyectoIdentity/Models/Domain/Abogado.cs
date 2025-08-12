using Microsoft.AspNetCore.Identity; // Necesario para IdentityUser
using ProyectoIdentity.Models.Enums;
using System.Collections.Generic; // Para colecciones de navegación
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace ProyectoIdentity.Models.Domain
{
    public class Abogado
    {
        [Key]
        public int AbogadoId { get; set; }

        [Required]
        [StringLength(450)] // Coincide con el tipo de Id de AspNetUsers
        public string UserId { get; set; } // FK a AspNetUsers.Id

        // Propiedad de navegación a AppUsuario
        [ForeignKey("UserId")]
        public AppUsuario AppUsuario { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } // Puedes decidir si mantenerlo aquí o usar AppUsuario.Nombre

        [StringLength(50)]
        [Display(Name = "Matrícula")]
        public string Matricula { get; set; }

        [Required(ErrorMessage = "El área de especialización es obligatoria.")]
        [Display(Name = "Área de Especialización")]
        public AreaEspecializacion AreaEspecializacion { get; set; }  // Podría ser un enum o FK a una tabla AreasPractica

        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Formato de email profesional inválido.")]
        [Display(Name = "Email Profesional")] 
        [Column("Email")] 
        public string Email { get; set; } 

        [StringLength(50)]
        [Display(Name = "Teléfono Profesional")] 
        [Column("Telefono")] 
        public string Telefono { get; set; } 

        public bool Activo { get; set; } = true; // Soft delete a nivel de usuario interno del estudio

        // Colecciones de navegación para las relaciones
        public ICollection<Caso> CasosAsignados { get; set; }
        public ICollection<Tarea> TareasAsignadas { get; set; }
        public ICollection<MovimientoFinanciero> MovimientosRegistrados { get; set; }
        public ICollection<RegistroTiempo> RegistrosDeTiempo { get; set; }
        public ICollection<Comunicacion> ComunicacionesRegistradas { get; set; }
    }
}