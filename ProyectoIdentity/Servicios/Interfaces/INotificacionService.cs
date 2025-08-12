using ProyectoIdentity.Models.Domain;

namespace ProyectoIdentity.Servicios.Interfaces
{
    public interface INotificacionService
    {
        Task CrearYNotificarAsync(string UserId, string mensaje);
        Task<IEnumerable<Notificacion>> GetAllNotificacionesAsync(string userId); // <-- Modificado
        Task<bool> MarcarNotificacionComoLeidaAsync(int notificacionId, string userId); // <-- NUEVO
    }
}