using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Datos;
using ProyectoIdentity.Hubs;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Servicios.Implementations;
using ProyectoIdentity.Servicios.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        private readonly INotificacionService _notificacionService; // <-- Inyectar el servicio
        private readonly UserManager<AppUsuario> _userManager;

        public NotificacionesController(INotificacionService notificacionService, UserManager<AppUsuario> userManager)
        {
            _notificacionService = notificacionService;
            _userManager = userManager;
        }


        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var usuarioId = _userManager.GetUserId(User); // Obtener el ID del usuario actual
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized(); // O un error más específico si no hay usuario
            }

            // Delegar al servicio para obtener las notificaciones no leídas
            var notificaciones = await _notificacionService.GetAllNotificacionesAsync(usuarioId); // Asumo que el servicio puede filtrar por usuarioId

            // Proyectar a un tipo anónimo que solo contenga el ID y el Mensaje para el frontend
            var notificacionesVm = notificaciones.Select(n => new { id = n.NotificacionId, mensaje = n.Mensaje, esLeida = n.EsLeida }).ToList();

            return Json(notificacionesVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            var usuarioId = _userManager.GetUserId(User); // Obtener el ID del usuario actual
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            // Delegar al servicio para marcar como leída
            var success = await _notificacionService.MarcarNotificacionComoLeidaAsync(id, usuarioId); // Asumo que el servicio implementa esto

            if (success)
            {
                // Opcional: Re-calcular y notificar el conteo de no leídas después de marcar una como leída
                // (Esto ya se hace en el JavaScript al remover el elemento y el servicio podría emitir al Hub si es necesario)
                // O si el servicio también actualiza el contador via Hub, no necesitas hacerlo aquí.

                return Ok();
            }
            return NotFound(); // O BadRequest si la notificación no pertenece al usuario o ya estaba leída
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(string mensaje)
        {
            var usuarioId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Unauthorized();
            }

            // Delegar la creación y notificación al servicio
            await _notificacionService.CrearYNotificarAsync(usuarioId, mensaje);

            return Ok(); // Devuelve 200 OK
        }

        // GET: Notificaciones/ObtenerConteoNoLeidas
        [HttpGet]
        public async Task<IActionResult> ObtenerConteoNoLeidas()
        {
            var usuarioId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(usuarioId))
            {
                return Json(0); // O Unauthorized()
            }
            var conteo = await _notificacionService.GetAllNotificacionesAsync(usuarioId);
            return Json(conteo.Count(n => !n.EsLeida)); // Contar las no leídas
        }
    }
}
