using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Datos;
using Microsoft.AspNetCore.SignalR;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        private readonly IHubContext<NotificacionesHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificacionesController(IHubContext<NotificacionesHub> hubContext, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _hubContext = hubContext;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var noti = await _context.Notificaciones.FindAsync(id);
            if (noti != null)
            {
                noti.Leida = true;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(string mensaje)
        {
            var usuarioId = _userManager.GetUserId(User); // o ClaimTypes.NameIdentifier
            var noti = new Notificacion
            {
                UsuarioId = usuarioId,
                Mensaje = mensaje,
                Fecha = DateTime.Now,
                Leida = false
            };
            _context.Notificaciones.Add(noti);
            await _context.SaveChangesAsync();

            Console.WriteLine("Notificación guardada y enviada a " + usuarioId);


            await _hubContext.Clients.User(usuarioId).SendAsync("RecibirNotificacion", mensaje);

            return Ok();
        }
    }
}
