using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos; // Cambia esto por tu namespace real
using ProyectoIdentity.Models.Domain; // Cambia esto también si es necesario



namespace ProyectoIdentity.ViewComponents
{
    public class NotificacionesViewComponent : ViewComponent
    {
        private readonly UserManager<AppUsuario> _userManager;
        private readonly ApplicationDbContext _context;

        public NotificacionesViewComponent(UserManager<AppUsuario> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var usuario = await _userManager.GetUserAsync(HttpContext.User);

            if (usuario == null)
            {
                return View(new List<Notificacion>()); // O una vista vacía
            }

            // Aquí se obtiene el conteo para el badge y/o la lista inicial si el ViewComponent renderiza el dropdown completo
            var notificaciones = await _context.Notificaciones
                .Where(n => n.UserId == usuario.Id && !n.EsLeida && !n.IsDeleted) // Filtrar también por IsDeleted
                .OrderByDescending(n => n.FechaNotificacion)
                .ToListAsync();

            // Si el ViewComponent solo devuelve el contador:
            // var conteoNoLeidas = await _context.Notificaciones.CountAsync(n => n.UserId == usuario.Id && !n.EsLeida);
            // return View(conteoNoLeidas);

            return View(notificaciones);
        }
    }
}
