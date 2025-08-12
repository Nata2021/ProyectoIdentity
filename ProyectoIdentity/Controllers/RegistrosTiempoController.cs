using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para IRegistroTiempoService, IAbogadoService, ICasoService
using ProyectoIdentity.Models.ViewModels; // Para RegistroTiempoCreateEditViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ

namespace ProyectoIdentity.Controllers
{
    // [Authorize(Roles = "Admin,Abogado,Secretario")] // Restringe el acceso
    public class RegistrosTiempoController : Controller
    {
        private readonly IRegistroTiempoService _registroTiempoService;
        private readonly IAbogadoService _abogadoService; // Para poblar dropdown de abogados
        private readonly ICasoService _casoService; // Para poblar dropdown de casos

        public RegistrosTiempoController(IRegistroTiempoService registroTiempoService, IAbogadoService abogadoService, ICasoService casoService)
        {
            _registroTiempoService = registroTiempoService;
            _abogadoService = abogadoService;
            _casoService = casoService;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(RegistroTiempoCreateEditViewModel model)
        {
            var abogados = await _abogadoService.GetAllAbogadosWithUserAsync();
            model.AbogadosList = abogados.Select(a => new SelectListItem
            {
                Value = a.AbogadoId.ToString(),
                Text = a.NombreCompleto
            }).OrderBy(li => li.Text);

            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})"
            }).OrderBy(li => li.Text);
        }

        // GET: RegistrosTiempo
        public async Task<IActionResult> Index()
        {
            var registros = await _registroTiempoService.GetAllRegistrosTiempoWithDetailsAsync();
            return View(registros);
        }

        // GET: RegistrosTiempo/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var registro = await _registroTiempoService.GetRegistroTiempoByIdWithDetailsAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            return View(registro);
        }

        // GET: RegistrosTiempo/Create
        public async Task<IActionResult> Create()
        {
            var model = new RegistroTiempoCreateEditViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: RegistrosTiempo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegistroTiempoCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var registroTiempo = new RegistroTiempo
                {
                    AbogadoId = model.AbogadoId,
                    CasoId = model.CasoId,
                    Fecha = model.Fecha,
                    Horas = model.Horas,
                    DescripcionActividad = model.DescripcionActividad,
                    Facturable = model.Facturable
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _registroTiempoService.AddRegistroTiempoAsync(registroTiempo);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: RegistrosTiempo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var registro = await _registroTiempoService.GetRegistroTiempoByIdWithDetailsAsync(id);
            if (registro == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new RegistroTiempoCreateEditViewModel
            {
                RegistroTiempoId = registro.RegistroTiempoId, // Importante para la edición
                AbogadoId = registro.AbogadoId,
                CasoId = registro.CasoId,
                Fecha = registro.Fecha,
                Horas = registro.Horas,
                DescripcionActividad = registro.DescripcionActividad,
                Facturable = registro.Facturable
            };
            await PopulateDropdowns(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: RegistrosTiempo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegistroTiempoCreateEditViewModel model)
        {
            if (id != model.RegistroTiempoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var registroToUpdate = await _registroTiempoService.GetRegistroTiempoByIdWithDetailsAsync(id); // Obtener la instancia a actualizar
                if (registroToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar las propiedades del registroToUpdate (la instancia rastreada)
                    registroToUpdate.AbogadoId = model.AbogadoId;
                    registroToUpdate.CasoId = model.CasoId;
                    registroToUpdate.Fecha = model.Fecha;
                    registroToUpdate.Horas = model.Horas;
                    registroToUpdate.DescripcionActividad = model.DescripcionActividad;
                    registroToUpdate.Facturable = model.Facturable;

                    await _registroTiempoService.UpdateRegistroTiempoAsync(registroToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _registroTiempoService.RegistroTiempoExistsAsync(id))
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

        // GET: RegistrosTiempo/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _registroTiempoService.GetRegistroTiempoByIdWithDetailsAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            return View(registro);
        }

        // POST: RegistrosTiempo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _registroTiempoService.DeleteRegistroTiempoAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}