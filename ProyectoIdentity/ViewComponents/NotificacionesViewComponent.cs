using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos; // Cambia esto por tu namespace real
using ProyectoIdentity.Models; // Cambia esto también si es necesario



namespace ProyectoIdentity.ViewComponents
{
    public class NotificacionesViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public NotificacionesViewComponent(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var usuario = await _userManager.GetUserAsync(HttpContext.User);

            if (usuario == null)
            {
                return View(new List<Notificacion>());
            }

            var notificaciones = await _context.Notificaciones
                .Where(n => n.UsuarioId == usuario.Id && !n.Leida)
                .OrderByDescending(n => n.Fecha)
                .ToListAsync();

            return View(notificaciones);
        }
    }
}
