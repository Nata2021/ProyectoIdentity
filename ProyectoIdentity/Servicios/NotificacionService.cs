using Microsoft.AspNetCore.SignalR;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Models;

namespace ProyectoIdentity.Servicios
{
    public class NotificacionService : INotificacionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificacionesHub> _hubContext;

        public NotificacionService(ApplicationDbContext context, IHubContext<NotificacionesHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task CrearYNotificarAsync(string usuarioId, string mensaje)
        {
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Mensaje = mensaje,
                Fecha = DateTime.Now,
                Leida = false
            };

            _context.Notificaciones.Add(notificacion);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(usuarioId).SendAsync("RecibirNotificacion", mensaje);
        }
    }
}
