using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Models.Domain.Enums;
using ProyectoIdentity.Models.ViewModels;
using ProyectoIdentity.Services.Implementations;
using ProyectoIdentity.Services.Interfaces; // Para los servicios que vamos a usar
using ProyectoIdentity.Servicios.Implementations;
using ProyectoIdentity.Servicios.Interfaces;
using System.Diagnostics;
using System.Linq; // Para LINQ
using System.Threading.Tasks; // Para Task

namespace ProyectoIdentity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly SignInManager<AppUsuario> _signInManager;
        private readonly ICasoService _casoService;
        private readonly ITareaService _tareaService;
        private readonly IAudienciaService _audienciaService;
        private readonly INotificacionService _notificacionService; // Asumiendo que ya tienes este servicio

        public HomeController(ILogger<HomeController> logger, UserManager<AppUsuario> userManager, SignInManager<AppUsuario> signInManager, ICasoService casoService,
                              ITareaService tareaService,
                              IAudienciaService audienciaService,
                              INotificacionService notificacionService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _casoService = casoService;
            _tareaService = tareaService;
            _audienciaService = audienciaService;
            _notificacionService = notificacionService;
        }

        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
            {
                ViewData["Autenticacion2FactoresActiva"] = false;
            }
            else
            {
                ViewData["Autenticacion2FactoresActiva"] = usuario.TwoFactorEnabled;
            }

            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // NUEVA ACCIÓN: Dashboard
        [Authorize] // Solo usuarios autenticados pueden acceder
        public async Task<IActionResult> Dashboard()
        {
            var currentUserId = _userManager.GetUserId(User); // Obtener el ID del usuario actual

            var model = new DashboardViewModel();

            // 1. Contadores Clave
            var todosLosCasos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            model.TotalCasosActivos = todosLosCasos.Count(c => c.Estado == EstadoCaso.Activo && !c.IsDeleted);

            var todasLasTareas = await _tareaService.GetAllTareasWithDetailsAsync();
            // Si el usuario es un abogado, solo sus tareas
            var tareasParaUsuario = User.IsInRole("Abogado")
                ? todasLasTareas.Where(t => t.AbogadoAsignado.UserId == currentUserId)
                : todasLasTareas; // Administrador o Secretario ve todas

            model.TotalTareasPendientes = tareasParaUsuario.Count(t => t.Estado == EstadoTarea.Pendiente && !t.IsDeleted);

            var todasLasAudiencias = await _audienciaService.GetAllAudienciasWithCaseDetailsAsync();
            var proximasAudiencias = todasLasAudiencias
                .Where(a => a.Fecha >= DateTime.Today && !a.IsDeleted)
                .OrderBy(a => a.Fecha);

            model.TotalProximasAudiencias = proximasAudiencias.Count();

            var usuarioId = _userManager.GetUserId(User); // Obtener el ID del usuario actual

            
                var todasLasNotificaciones = await _notificacionService.GetAllNotificacionesAsync(usuarioId);
                ViewBag.Notificaciones = todasLasNotificaciones;

           
                // Si el usuario no está logueado, puedes pasar una lista vacía o manejarlo según tu lógica
                ViewBag.Notificaciones = new List<Notificacion>();         

             // O pásalas al modelo de tu vista
            var notificacionesParaUsuario = todasLasNotificaciones

                 .Where(n => n.UserId == currentUserId && !n.IsDeleted);
            model.TotalNotificacionesNoLeidas = notificacionesParaUsuario.Count(n => !n.EsLeida);

            // 2. Listas de Elementos Importantes (Ej. Top 5 o 10)
            model.MisProximasAudiencias = proximasAudiencias
                    .Where(a => a.Caso.Abogado.UserId == currentUserId || User.IsInRole("Administrador") || User.IsInRole("Secretario")) // Audiencias del abogado actual o todas
                    .Take(5); // Las 5 más próximas

                model.MisTareasPendientes = tareasParaUsuario
                    .Where(t => t.Estado == EstadoTarea.Pendiente)
                    .OrderBy(t => t.FechaLimite)
                    .Take(5); // Las 5 tareas pendientes más urgentes

                model.UltimasNotificaciones = notificacionesParaUsuario
                    .OrderByDescending(n => n.FechaNotificacion)
                    .Take(5); // Las 5 últimas notificaciones

                model.CasosRecientementeActualizados = todosLosCasos
                    .Where(c => !c.IsDeleted && c.Abogado.UserId == currentUserId || User.IsInRole("Administrador") || User.IsInRole("Secretario"))
                    .OrderByDescending(c => c.FechaUltimaActualizacion ?? c.FechaCreacion) // Si no hay fecha de actualización, usa la de creación
                    .Take(5);

                return View(model);
            }
        
       
    }
}