using Microsoft.AspNetCore.Mvc;
using ProyectoIdentity.Models.Domain;
using ProyectoIdentity.Services.Interfaces; // Para IMovimientoFinancieroService, ICasoService, IAbogadoService
using ProyectoIdentity.Models.ViewModels; // Para MovimientoFinancieroCreateEditViewModel
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore; // Para DbUpdateConcurrencyException
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectListItem
using System.Linq; // Para LINQ

namespace ProyectoIdentity.Controllers
{
    // [Authorize(Roles = "Admin,Secretario,Abogado")] // Restringe el acceso
    public class MovimientosFinancierosController : Controller
    {
        private readonly IMovimientoFinancieroService _movimientoService;
        private readonly ICasoService _casoService; // Para poblar dropdown de casos
        private readonly IAbogadoService _abogadoService; // Para poblar dropdown de abogados

        public MovimientosFinancierosController(IMovimientoFinancieroService movimientoService, ICasoService casoService, IAbogadoService abogadoService)
        {
            _movimientoService = movimientoService;
            _casoService = casoService;
            _abogadoService = abogadoService;
        }

        // Helper para poblar los SelectLists
        private async Task PopulateDropdowns(MovimientoFinancieroCreateEditViewModel model)
        {
            var casos = await _casoService.GetAllCasosWithClienteAndAbogadoAsync();
            model.CasosList = casos.Select(c => new SelectListItem
            {
                Value = c.CasoId.ToString(),
                Text = $"{c.Titulo} ({c.Cliente.NombreCompleto})"
            }).OrderBy(li => li.Text);

            var abogados = await _abogadoService.GetAllAbogadosWithUserAsync();
            model.AbogadosList = abogados.Select(a => new SelectListItem
            {
                Value = a.AbogadoId.ToString(),
                Text = a.NombreCompleto
            }).OrderBy(li => li.Text);
        }

        // GET: MovimientosFinancieros
        public async Task<IActionResult> Index()
        {
            var movimientos = await _movimientoService.GetAllMovimientosWithDetailsAsync();
            return View(movimientos);
        }

        // GET: MovimientosFinancieros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movimiento = await _movimientoService.GetMovimientoByIdWithDetailsAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            return View(movimiento);
        }

        // GET: MovimientosFinancieros/Create
        public async Task<IActionResult> Create()
        {
            var model = new MovimientoFinancieroCreateEditViewModel();
            await PopulateDropdowns(model); // Rellenar los desplegables
            return View(model);
        }

        // POST: MovimientosFinancieros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovimientoFinancieroCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movimiento = new MovimientoFinanciero
                {
                    CasoId = model.CasoId,
                    TipoMovimiento = model.TipoMovimiento,
                    Concepto = model.Concepto,
                    Monto = model.Monto,
                    FechaMovimiento = model.FechaMovimiento,
                    AbogadoId = model.AbogadoId
                    // IsDeleted ya tiene un valor por defecto en el modelo
                };

                await _movimientoService.AddMovimientoAsync(movimiento);
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(model); // Vuelve a rellenar si el modelo no es válido
            return View(model);
        }

        // GET: MovimientosFinancieros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movimiento = await _movimientoService.GetMovimientoByIdWithDetailsAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            // Mapear el modelo de dominio a nuestro ViewModel de edición
            var model = new MovimientoFinancieroCreateEditViewModel
            {
                MovimientoId = movimiento.MovimientoId, // Importante para la edición
                CasoId = movimiento.CasoId,
                TipoMovimiento = movimiento.TipoMovimiento,
                Concepto = movimiento.Concepto,
                Monto = movimiento.Monto,
                FechaMovimiento = movimiento.FechaMovimiento,
                AbogadoId = movimiento.AbogadoId
            };
            await PopulateDropdowns(model); // Rellenar los desplegables

            return View(model);
        }

        // POST: MovimientosFinancieros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovimientoFinancieroCreateEditViewModel model)
        {
            if (id != model.MovimientoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var movimientoToUpdate = await _movimientoService.GetMovimientoByIdWithDetailsAsync(id); // Obtener la instancia a actualizar
                if (movimientoToUpdate == null)
                {
                    return NotFound();
                }

                try
                {
                    // Actualizar las propiedades del movimientoToUpdate (la instancia rastreada)
                    movimientoToUpdate.CasoId = model.CasoId;
                    movimientoToUpdate.TipoMovimiento = model.TipoMovimiento;
                    movimientoToUpdate.Concepto = model.Concepto;
                    movimientoToUpdate.Monto = model.Monto;
                    movimientoToUpdate.FechaMovimiento = model.FechaMovimiento;
                    movimientoToUpdate.AbogadoId = model.AbogadoId;

                    await _movimientoService.UpdateMovimientoAsync(movimientoToUpdate);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _movimientoService.MovimientoExistsAsync(id))
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

        // GET: MovimientosFinancieros/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movimiento = await _movimientoService.GetMovimientoByIdWithDetailsAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            return View(movimiento);
        }

        // POST: MovimientosFinancieros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movimientoService.DeleteMovimientoAsync(id); // Esto hará un soft delete
            return RedirectToAction(nameof(Index));
        }
    }
}