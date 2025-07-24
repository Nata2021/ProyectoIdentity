using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Models;
using ProyectoIdentity.Servicios;
using System.Text.Encodings.Web;

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _contexto;
        private readonly INotificacionService _notificacionService;

        public RolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext contexto, INotificacionService notificacionService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _contexto = contexto;
            _notificacionService = notificacionService;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var roles = _contexto.Roles.ToList();
            return View(roles);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(IdentityRole rol)
        {
            if (await _roleManager.RoleExistsAsync(rol.Name))
            {
                return RedirectToAction(nameof(Index));
            }
            // Se crea el rol
            await _roleManager.CreateAsync(new IdentityRole() { Name = rol.Name });

            var usuarioId = _userManager.GetUserId(User);
            await _notificacionService.CrearYNotificarAsync(usuarioId, $"Se ha creado el rol '{rol.Name}' correctamente.");
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Editar(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View();
            }
            else
            {
                //Actualizar el rol
                var rolBD = _contexto.Roles.FirstOrDefault(r => r.Id == id);

                return View(rolBD);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(IdentityRole rol)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (await _roleManager.RoleExistsAsync(rol.Name))
            {

                await _notificacionService.CrearYNotificarAsync(usuarioId, $"El rol '{rol.Name}' ya existe.");
                return RedirectToAction(nameof(Index));
            }

            //Se crea el rol
            var rolBD = _contexto.Roles.FirstOrDefault(r => r.Id == rol.Id);
            if (rolBD == null)
            {
                return RedirectToAction(nameof(Index));
            }

            rolBD.Name = rol.Name;
            rolBD.NormalizedName = rol.Name.ToUpper();
            var resultado = await _roleManager.UpdateAsync(rolBD);

            await _notificacionService.CrearYNotificarAsync(usuarioId, $"Se ha editado el rol '{rol.Name}' correctamente.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Borrar(string id)
        {
            var usuarioId = _userManager.GetUserId(User);

            var rolBD = _contexto.Roles.FirstOrDefault(r => r.Id == id);
            if (rolBD == null)
            {

                await _notificacionService.CrearYNotificarAsync(usuarioId, $"No exite el rol '{rolBD.Name}'.");
                return RedirectToAction(nameof(Index));
            }

            var usuariosParaEsteRol = _contexto.UserRoles.Where(u => u.RoleId == id).Count();
            if (usuariosParaEsteRol > 0)
            {

                await _notificacionService.CrearYNotificarAsync(usuarioId, $"El rol '{rolBD.Name}' tiene usuarios asociados, no se puede borrar.");
                return RedirectToAction(nameof(Index));
            }

            await _roleManager.DeleteAsync(rolBD);

            await _notificacionService.CrearYNotificarAsync(usuarioId, $"Rol '{rolBD.Name}' borrado correctamente.");
            return RedirectToAction(nameof(Index));
        }
    }
}
