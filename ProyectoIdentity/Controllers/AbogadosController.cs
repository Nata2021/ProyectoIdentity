using Microsoft.AspNetCore.Authorization; // Para autorizar acceso
using Microsoft.AspNetCore.Identity; // Para UserManager, si necesitas el usuario actual
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoIdentity.Models.Domain; // Para el modelo Abogado
using ProyectoIdentity.Models.ViewModels;
using ProyectoIdentity.Services.Interfaces; // Para IAbogadoService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador")] // Solo administradores o abogados pueden acceder
    public class AbogadosController : Controller
    {
        private readonly IAbogadoService _abogadoService;
        private readonly UserManager<AppUsuario> _userManager; // Para gestionar usuarios de Identity

        public AbogadosController(IAbogadoService abogadoService, UserManager<AppUsuario> userManager)
        {
            _abogadoService = abogadoService;
            _userManager = userManager;
        }

        // GET: Abogados
        public async Task<IActionResult> Index()
        {
            // Usamos el método que incluye el usuario
            var abogados = await _abogadoService.GetAllAbogadosWithUserAsync();
            return View(abogados);
        }

        // GET: Abogados/Details/5
        // Puedes fusionar esto en Edit o tener una vista solo para ver
        public async Task<IActionResult> Details(int id)
        {
            // Usamos el método que incluye el usuario
            var abogado = await _abogadoService.GetAbogadoByIdWithUserAsync(id);
            if (abogado == null)
            {
                return NotFound();
            }
            return View(abogado);
        }

        // GET: Abogados/Create
        [HttpGet]      
        public async Task<IActionResult> Create()
        {
            // Obtener todos los usuarios para el dropdown
            var usuarios = await _userManager.Users.ToListAsync();
            var model = new AbogadoCreateViewModel
            {
                UsuariosList = usuarios.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.Email} ({u.Nombre})" // Puedes mostrar Email y Nombre para identificar
                })
            };
            return View(model);
        }

        // POST: Abogados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AbogadoCreateViewModel model) // Ahora recibe el ViewModel
        {
            if (ModelState.IsValid)
            {
                // Verificar si el UserId ya está asociado a otro abogado (regla de negocio: 1 usuario -> 1 abogado)
                if (await _abogadoService.AbogadoUserIdExistsAsync(model.UserId))
                {
                    ModelState.AddModelError("UserId", "Este usuario ya está asociado a otro abogado.");
                    // Vuelve a poblar el dropdown antes de retornar la vista
                    model.UsuariosList = (await _userManager.Users.ToListAsync()).Select(u => new SelectListItem
                    {
                        Value = u.Id,
                        Text = $"{u.Email} ({u.Nombre})"
                    });
                    return View(model);
                }

                // Crear la entidad Abogado a partir del ViewModel
                var abogado = new Abogado
                {
                    UserId = model.UserId,
                    NombreCompleto = model.NombreCompleto,
                    Matricula = model.Matricula,
                    AreaEspecializacion = model.AreaEspecializacion,
                    Email = model.Email,    
                    Telefono = model.Telefono, 
                    Activo = model.Activo
                };

                await _abogadoService.AddAbogadoAsync(abogado);
                TempData["Correcto"] = "Abogado creado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, vuelve a poblar el dropdown y retorna la vista
            model.UsuariosList = (await _userManager.Users.ToListAsync()).Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = $"{u.Email} ({u.Nombre})"
            });
            return View(model);
        }

        // GET: Abogados/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // Usamos el método que incluye el usuario
            var abogado = await _abogadoService.GetAbogadoByIdWithUserAsync(id);
            if (abogado == null)
            {
                return NotFound();
            }
            return View(abogado);
        }

        // POST: Abogados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Abogado abogado) // Aquí podrías usar un ViewModel
        {
            if (id != abogado.AbogadoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _abogadoService.UpdateAbogadoAsync(abogado);
                    TempData["Correcto"] = "Abogado actualizado correctamente.";
                }
                catch (DbUpdateConcurrencyException) // Si usas EF Core y hay problemas de concurrencia
                {
                    if (!await _abogadoService.AbogadoExistsAsync(abogado.AbogadoId))
                    {
                        TempData["Error"] = "Error: Abogado no encontrado para actualizar.";
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Error de concurrencia al actualizar el abogado.";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(abogado);
        }

        // GET: Abogados/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var abogado = await _abogadoService.GetAbogadoByIdAsync(id);
            if (abogado == null)
            {
                return NotFound();
            }
            return View(abogado); // O una vista de confirmación de eliminación
        }

        // POST: Abogados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _abogadoService.DeleteAbogadoAsync(id); // Esto hará un soft delete (marcar Activo=false)
            TempData["Correcto"] = "Abogado eliminado correctamente (estado inactivo).";
            return RedirectToAction(nameof(Index));
        }
    }
}