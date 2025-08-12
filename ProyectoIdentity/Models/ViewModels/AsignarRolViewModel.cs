// ProyectoIdentity/Models/ViewModels/AsignarRolViewModel.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Collections.Generic;

namespace ProyectoIdentity.Models.ViewModels
{
    public class AsignarRolViewModel
    {
        public string UserId { get; set; }

        [Display(Name = "Usuario")]
        public string UserName { get; set; }

        [Display(Name = "Roles Asignados")]
        public IEnumerable<string> RolesAsignados { get; set; } // Nombres de los roles actuales

        [Display(Name = "Roles Disponibles")]
        public IEnumerable<SelectListItem> TodosLosRoles { get; set; } // Todos los roles para el CheckboxList

        // Propiedad para la selección del formulario
        [Display(Name = "Seleccionar Roles")]
        public List<string> RolesSeleccionados { get; set; } // Nombres de los roles seleccionados por el usuario
    }
}