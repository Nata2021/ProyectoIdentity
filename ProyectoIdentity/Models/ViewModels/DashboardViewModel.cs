using ProyectoIdentity.Models.Domain; // Para los modelos de dominio
using ProyectoIdentity.Models.Domain.Enums; // Para los enums
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoIdentity.Models.ViewModels
{
    public class DashboardViewModel
    {
        // Contadores clave
        [Display(Name = "Casos Activos")]
        public int TotalCasosActivos { get; set; }

        [Display(Name = "Tareas Pendientes")]
        public int TotalTareasPendientes { get; set; }

        [Display(Name = "Próximas Audiencias")]
        public int TotalProximasAudiencias { get; set; }

        [Display(Name = "Notificaciones No Leídas")]
        public int TotalNotificacionesNoLeidas { get; set; }

        // Listas de elementos importantes (Top X)
        [Display(Name = "Mis Próximas Audiencias")]
        public IEnumerable<Audiencia> MisProximasAudiencias { get; set; }

        [Display(Name = "Mis Tareas Pendientes")]
        public IEnumerable<Tarea> MisTareasPendientes { get; set; }

        [Display(Name = "Últimas Notificaciones")]
        public IEnumerable<Notificacion> UltimasNotificaciones { get; set; }

        [Display(Name = "Casos Recientemente Actualizados")]
        public IEnumerable<Caso> CasosRecientementeActualizados { get; set; }

        // Puedes añadir más métricas o listas según lo necesites
        // Por ejemplo: total de clientes, total de abogados, resumen financiero, etc.
    }
}