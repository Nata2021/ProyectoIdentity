using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Models;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Repositories.Interfaces;
using ProyectoIdentity.Servicios.Interfaces;

namespace ProyectoIdentity.Servicios.Implementations
{
    public class NotificacionService : INotificacionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificacionesHub> _hubContext;
        private readonly IRepository<Notificacion> _notificacionRepository;

        public NotificacionService(ApplicationDbContext context, IHubContext<NotificacionesHub> hubContext, IRepository<Notificacion> notificacionRepository)
        {
            _context = context;
            _hubContext = hubContext;
            _notificacionRepository = notificacionRepository;
        }

        public async Task CrearYNotificarAsync(string usuarioId, string mensaje)
        {
            var notificacion = new Notificacion
            {
                UserId = usuarioId,
                Mensaje = mensaje,
                FechaNotificacion = DateTime.Now,
                EsLeida = false
            };

            await _notificacionRepository.AddAsync(notificacion); // Usamos el repositorio
            // _context.Notificaciones.Add(notificacion); // Ya no es necesario si usas el repositorio
            // await _context.SaveChangesAsync();          // Ya no es necesario si usas el repositorio

            // Opcional: Notificar al cliente que hay una nueva notificación y el contador.
            // Aquí podrías enviar el ID de la notificación también para que el JS lo use.
            await _hubContext.Clients.User(usuarioId).SendAsync("RecibirNotificacion", notificacion.Mensaje, notificacion.NotificacionId);

            // Actualizar el contador de notificaciones no leídas para el usuario
            var conteoNoLeidas = await _context.Notificaciones.CountAsync(n => n.UserId == usuarioId && !n.EsLeida);
            await _hubContext.Clients.User(usuarioId).SendAsync("ActualizarContador", conteoNoLeidas);
        }

       // MODIFICADO: Ahora filtra por UserId
        public async Task<IEnumerable<Notificacion>> GetAllNotificacionesAsync(string userId)
        {
            return await _notificacionRepository.FindAsync(n => n.UserId == userId && !n.IsDeleted);
        }



        // NUEVO: Implementación para marcar como leída
        public async Task<bool> MarcarNotificacionComoLeidaAsync(int notificacionId, string userId)
        {
            var notificacion = await _notificacionRepository.GetSingleOrDefaultWithIncludesAsync(
                n => n.NotificacionId == notificacionId && n.UserId == userId && !n.IsDeleted);

            if (notificacion == null || notificacion.EsLeida)
            {
                return false; // Notificación no encontrada, no pertenece al usuario o ya está leída
            }

            notificacion.EsLeida = true;
            await _notificacionRepository.UpdateAsync(notificacion);

            // Opcional: Re-calcular y notificar el conteo de no leídas después de marcar una como leída
            var conteoNoLeidas = await _context.Notificaciones.CountAsync(n => n.UserId == userId && !n.EsLeida);
            await _hubContext.Clients.User(userId).SendAsync("ActualizarContador", conteoNoLeidas);

            return true;
        }
    }
}
