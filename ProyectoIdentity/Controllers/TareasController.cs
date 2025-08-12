using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity; // Para UserManager para CreadaPorUserId
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Models.Domain.Enums;
using ProyectoIdentity.Models.ViewModels; // Para TareaCreateEditViewModel
using ProyectoIdentity.Services.Interfaces; // Para ITareaService, ICasoService, IAbogadoService
using System.Collections.Generic;
using System.Linq; // Para LINQ
using System.Threading.Tasks;

namespace ProyectoIdentity.Controllers
{
    // [Authorize(Roles = "Admin,Abogado,Secretario")] // Restringe el acceso
    public class TareasController : Controller
    {
        private readonly ITareaService _tareaService;
        private readonly ICasoService _casoService; // Para poblar dropdown de casos
        private readonly IAbogadoService _abogadoService; // Para poblar dropdown de abogados
        private readonly UserManager<AppUsuario> _userManager; // Para obtener el usuario actual que crea la tarea

        public TareasController(ITareaService tareaService, ICasoService casoService, IAbogadoService abogadoService, UserManager<AppUsuario> userManager)
        {
            _tareaService = tareaService;
            _casoService = casoService;
            _abogadoService = abogadoService;
            _userManager = userManager;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(TareaCreateEditViewModel model)
        {
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync(); // Obtenemos todos los casos
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})" // Título del caso (Nombre del Cliente)
            }).OrderBy(li => li.Text);

            var abogados = await _abogadoService.GetAllAbogadosWithUserAsync(); // Obtenemos todos los abogados
            model.AbogadosList = abogados.Select(a => new SelectListItem
            {
                Value = a.AbogadoId.ToString(),
                Text = a.NombreCompleto
            }).OrderBy(li => li.Text);
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var tareas = await _tareaService.GetAllTareasWithDetailsAsync();
            return View(tareas);
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var tarea = await _tareaService.GetTareaByIdWithDetailsAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        // GET: Tareas/Create
        public async Task<IActionResult> Create()
        {
            var model = new TareaCreateEditViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TareaCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var tarea = new Tarea
                {
                    CasoId = model.CasoId,
                    Descripcion = model.Descripcion,
                    FechaLimite = model.FechaLimite,
                    Estado = model.Estado,
                    AsignadoA = model.AsignadoA,
                    TipoTarea = model.TipoTarea,
                    Prioridad = model.Prioridad,
                    PorcentajeProgreso = model.PorcentajeProgreso,
                    CreadaPorUserId = currentUser?.Id, // Asignar el ID del usuario actual
                    FechaCreacion = DateTime.Now // Asignar la fecha de creación
                };

                await _tareaService.AddTareaAsync(tarea);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Tareas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var tarea = await _tareaService.GetTareaByIdWithDetailsAsync(id); // Cargar la tarea con sus relaciones
            if (tarea == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new TareaCreateEditViewModel
            {
                TareaId = tarea.TareaId, // Importante para la edición
                CasoId = tarea.CasoId,
                Descripcion = tarea.Descripcion,
                FechaLimite = tarea.FechaLimite,
                Estado = tarea.Estado,
                AsignadoA = tarea.AsignadoA,
                TipoTarea = tarea.TipoTarea,
                Prioridad = tarea.Prioridad,
                PorcentajeProgreso = tarea.PorcentajeProgreso
            };
            await PopulateDropdowns(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TareaCreateEditViewModel model)
        {
            if (id != model.TareaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var tareaToUpdate = await _tareaService.GetTareaByIdWithDetailsAsync(id); // Obtener la instancia a actualizar
                if (tareaToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar las propiedades del tareaToUpdate (la instancia rastreada)
                    tareaToUpdate.CasoId = model.CasoId;
                    tareaToUpdate.Descripcion = model.Descripcion;
                    tareaToUpdate.FechaLimite = model.FechaLimite;
                    tareaToUpdate.Estado = model.Estado;
                    tareaToUpdate.AsignadoA = model.AsignadoA;
                    tareaToUpdate.TipoTarea = model.TipoTarea;
                    tareaToUpdate.Prioridad = model.Prioridad;
                    tareaToUpdate.PorcentajeProgreso = model.PorcentajeProgreso;
                    // FechaUltimaActualizacion se actualiza en el modelo o en el servicio si se implementó

                    // Si implementaste FechaCompletado:
                    if (model.Estado == EstadoTarea.Completada && tareaToUpdate.FechaCompletado == null)
                    {
                        tareaToUpdate.FechaCompletado = DateTime.Now;
                    }
                    else if (model.Estado != EstadoTarea.Completada && tareaToUpdate.FechaCompletado != null)
                    {
                        tareaToUpdate.FechaCompletado = null; // Si se descompleta
                    }


                    await _tareaService.UpdateTareaAsync(tareaToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _tareaService.TareaExistsAsync(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Tareas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var tarea = await _tareaService.GetTareaByIdWithDetailsAsync(id);
            if (tarea == null)
            {
                return NotFound();
            }
            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _tareaService.DeleteTareaAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}