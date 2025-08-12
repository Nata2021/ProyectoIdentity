// ProyectoIdentity/Models/ViewModels/UsuarioListViewModel.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ProyectoIdentity.Models.Domain; // Para AppUsuario
using Microsoft.AspNetCore.Identity; // Para IdentityRole

namespace ProyectoIdentity.Models.ViewModels
{
    public class UsuarioListViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Nombre Completo")]
        public string Nombre { get; set; } // De tu AppUsuario

        [Display(Name = "Estado (Activo)")]
        public bool Estado { get; set; } // De tu AppUsuario

        [Display(Name = "Roles")]
        public string Roles { get; set; } // Cadena con los roles separados por coma
    }
}