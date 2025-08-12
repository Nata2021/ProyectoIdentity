using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para ICasoService, IClienteService, IAbogadoService
using ProyectoIdentity.Models.ViewModels; // Para CasoCreateViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ

namespace ProyectoIdentity.Controllers
{
    [Authorize(Roles = "Administrador,Abogado,Secretario")] // Restringe el acceso
    public class CasosController : Controller
    {
        private readonly ICasoService _casoService;
        private readonly IClienteService _clienteService; // Para poblar el dropdown de clientes
        private readonly IAbogadoService _abogadoService; // Para poblar el dropdown de abogados

        public CasosController(ICasoService casoService, IClienteService clienteService, IAbogadoService abogadoService)
        {
            _casoService = casoService;
            _clienteService = clienteService;
            _abogadoService = abogadoService;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(CasoCreateViewModel model)
        {
            var clientes = await _clienteService.GetAllClientesAsync();
            model.ClientesList = clientes.Select(c => new SelectListItem
            {
                Value = c.ClienteId.ToString(),
                Text = c.NombreCompleto
            }).OrderBy(li => li.Text); // Ordenar alfabéticamente

            var abogados = await _abogadoService.GetAllAbogadosAsync(); // Asegúrate de que este método carga el nombre completo
            model.AbogadosList = abogados.Select(a => new SelectListItem
            {
                Value = a.AbogadoId.ToString(),
                Text = a.NombreCompleto // Usamos NombreCompleto del abogado
            }).OrderBy(li => li.Text); // Ordenar alfabéticamente
        }


        // GET: Casos
        public async Task<IActionResult> Index()
        {
            // Usamos el método que incluye Cliente y Abogado para mostrar sus nombres
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            return View(casos);
        }

        // GET: Casos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var caso = await _casoService.GetCasoByIdWithClienteAndAbogadoAsync(id);
            if (caso == null)
            {
                return NotFound();
            }
            return View(caso);
        }

        // GET: Casos/Create
        public async Task<IActionResult> Create()
        {
            var model = new CasoCreateViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: Casos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CasoCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var caso = new Caso
                {
                    Titulo = model.Titulo,
                    Descripcion = model.Descripcion,
                    FechaInicio = model.FechaInicio,
                    Estado = model.Estado,
                    ClienteId = model.ClienteId,
                    AbogadoId = model.AbogadoId,
                    TipoCaso = model.TipoCaso,
                    Prioridad = model.Prioridad,
                    FechaCreacion = DateTime.Now // Se asigna en el código
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _casoService.AddCasoAsync(caso);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Casos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var caso = await _casoService.GetCasoByIdWithClienteAndAbogadoAsync(id); // Cargar el caso con sus relaciones
            if (caso == null)
            {
                return NotFound();
            }
            // Puedes usar un CasoEditViewModel si la lógica de edición es diferente a la de creación
            // Por ahora, pasamos el modelo de dominio.

            // Para la vista de edición, también necesitamos los desplegables
            var model = new CasoCreateViewModel // Reutilizamos el ViewModel de creación para poblar los desplegables
            {
                Titulo = caso.Titulo,
                Descripcion = caso.Descripcion,
                FechaInicio = caso.FechaInicio,
                Estado = caso.Estado,
                ClienteId = caso.ClienteId,
                AbogadoId = caso.AbogadoId,
                TipoCaso = caso.TipoCaso,
                Prioridad = caso.Prioridad
            };
            await PopulateDropdowns(model);
            ViewData["CasoId"] = caso.CasoId; // Pasamos el ID para el hidden field en la vista
            return View(model);
        }

        // POST: Casos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CasoCreateViewModel model) // Recibe el ViewModel de creación
        {
            if (id != (int)ViewData["CasoId"]) // Para asegurar que el ID en la URL coincide con el del hidden field
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var casoToUpdate = await _casoService.GetCasoByIdWithClienteAndAbogadoAsync(id); // Obtener la instancia a actualizar
                if (casoToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    casoToUpdate.Titulo = model.Titulo;
                    casoToUpdate.Descripcion = model.Descripcion;
                    casoToUpdate.FechaInicio = model.FechaInicio;
                    casoToUpdate.Estado = model.Estado;
                    casoToUpdate.ClienteId = model.ClienteId;
                    casoToUpdate.AbogadoId = model.AbogadoId;
                    casoToUpdate.TipoCaso = model.TipoCaso;
                    casoToUpdate.Prioridad = model.Prioridad;
                    casoToUpdate.FechaUltimaActualizacion = DateTime.Now; // Actualizar fecha de modificación

                    await _casoService.UpdateCasoAsync(casoToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _casoService.CasoExistsAsync(id))
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
            ViewData["CasoId"] = id; // Asegura que el ID se pasa de nuevo
            return View(model);
        }

        // GET: Casos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var caso = await _casoService.GetCasoByIdWithClienteAndAbogadoAsync(id);
            if (caso == null)
            {
                return NotFound();
            }
            return View(caso);
        }

        // POST: Casos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _casoService.DeleteCasoAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}