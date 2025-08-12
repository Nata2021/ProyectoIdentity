using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para IComunicacionService, ICasoService, IClienteService, IAbogadoService
using ProyectoIdentity.Models.ViewModels; // Para ComunicacionCreateEditViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ

namespace ProyectoIdentity.Controllers
{
    // [Authorize(Roles = "Admin,Secretario,Abogado")] // Restringe el acceso
    public class ComunicacionesController : Controller
    {
        private readonly IComunicacionService _comunicacionService;
        private readonly ICasoService _casoService; // Para poblar dropdown de casos
        private readonly IClienteService _clienteService; // Para poblar dropdown de clientes
        private readonly IAbogadoService _abogadoService; // Para poblar dropdown de abogados

        public ComunicacionesController(IComunicacionService comunicacionService, ICasoService casoService, IClienteService clienteService, IAbogadoService abogadoService)
        {
            _comunicacionService = comunicacionService;
            _casoService = casoService;
            _clienteService = clienteService;
            _abogadoService = abogadoService;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(ComunicacionCreateEditViewModel model)
        {
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})"
            }).OrderBy(li => li.Text);

            var clientes = await _clienteService.GetAllClientesAsync();
            model.ClientesList = clientes.Select(cl => new SelectListItem
            {
                Value = cl.ClienteId.ToString(),
                Text = $"{cl.NombreCompleto} ({cl.DNI_CUIT_CUIL})"
            }).OrderBy(li => li.Text);

            var abogados = await _abogadoService.GetAllAbogadosWithUserAsync();
            model.AbogadosList = abogados.Select(a => new SelectListItem
            {
                Value = a.AbogadoId.ToString(),
                Text = a.NombreCompleto
            }).OrderBy(li => li.Text);
        }

        // GET: Comunicaciones
        public async Task<IActionResult> Index()
        {
            var comunicaciones = await _comunicacionService.GetAllComunicacionesWithDetailsAsync();
            return View(comunicaciones);
        }

        // GET: Comunicaciones/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var comunicacion = await _comunicacionService.GetComunicacionByIdWithDetailsAsync(id);
            if (comunicacion == null)
            {
                return NotFound();
            }
            return View(comunicacion);
        }

        // GET: Comunicaciones/Create
        public async Task<IActionResult> Create()
        {
            var model = new ComunicacionCreateEditViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: Comunicaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComunicacionCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var comunicacion = new Comunicacion
                {
                    CasoId = model.CasoId,
                    ClienteId = model.ClienteId,
                    AbogadoId = model.AbogadoId,
                    TipoComunicacion = model.TipoComunicacion,
                    Asunto = model.Asunto,
                    Detalle = model.Detalle,
                    FechaComunicacion = model.FechaComunicacion
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _comunicacionService.AddComunicacionAsync(comunicacion);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: Comunicaciones/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var comunicacion = await _comunicacionService.GetComunicacionByIdWithDetailsAsync(id);
            if (comunicacion == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new ComunicacionCreateEditViewModel
            {
                ComunicacionId = comunicacion.ComunicacionId, // Importante para la edición
                CasoId = comunicacion.CasoId,
                ClienteId = comunicacion.ClienteId,
                AbogadoId = comunicacion.AbogadoId,
                TipoComunicacion = comunicacion.TipoComunicacion,
                Asunto = comunicacion.Asunto,
                Detalle = comunicacion.Detalle,
                FechaComunicacion = comunicacion.FechaComunicacion
            };
            await PopulateDropdowns(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: Comunicaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ComunicacionCreateEditViewModel model)
        {
            if (id != model.ComunicacionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var comunicacionToUpdate = await _comunicacionService.GetComunicacionByIdWithDetailsAsync(id); // Obtener la instancia a actualizar
                if (comunicacionToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar las propiedades del comunicacionToUpdate (la instancia rastreada)
                    comunicacionToUpdate.CasoId = model.CasoId;
                    comunicacionToUpdate.ClienteId = model.ClienteId;
                    comunicacionToUpdate.AbogadoId = model.AbogadoId;
                    comunicacionToUpdate.TipoComunicacion = model.TipoComunicacion;
                    comunicacionToUpdate.Asunto = model.Asunto;
                    comunicacionToUpdate.Detalle = model.Detalle;
                    comunicacionToUpdate.FechaComunicacion = model.FechaComunicacion;

                    await _comunicacionService.UpdateComunicacionAsync(comunicacionToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _comunicacionService.ComunicacionExistsAsync(id))
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

        // GET: Comunicaciones/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var comunicacion = await _comunicacionService.GetComunicacionByIdWithDetailsAsync(id);
            if (comunicacion == null)
            {
                return NotFound();
            }
            return View(comunicacion);
        }

        // POST: Comunicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _comunicacionService.DeleteComunicacionAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}