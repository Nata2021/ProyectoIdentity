using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic; // Para IEnumerable
using ProyectoIdentity.Models.Domain.Enums; // Para los enums de Caso

namespace ProyectoIdentity.Models.ViewModels
{
    public class CasoCreateViewModel
    {
        [Required(ErrorMessage = "El título del caso es obligatorio.")]
        [StringLength(255)]
        [Display(Name = "Título del Caso")]
        public string Titulo { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Today;

        [Display(Name = "Estado")]
        public EstadoCaso Estado { get; set; } = EstadoCaso.Activo; // Valor por defecto

        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public IEnumerable<SelectListItem> ClientesList { get; set; } // Para el desplegable

        [Required(ErrorMessage = "Debe seleccionar un abogado.")]
        [Display(Name = "Abogado Asignado")]
        public int AbogadoId { get; set; }
        public IEnumerable<SelectListItem> AbogadosList { get; set; } // Para el desplegable

        [Display(Name = "Tipo de Caso")]
        public TipoCaso TipoCaso { get; set; } = TipoCaso.Civil; // Valor por defecto

        [Display(Name = "Prioridad")]
        public Prioridad Prioridad { get; set; } = Prioridad.Media; // Valor por defecto

        // IsDeleted no se expone en la creación, es para soft delete interno
    }
}